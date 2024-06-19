using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
    internal class HlbvhBuilder
    {
        private ComputeShader shaderBuildHlbvh;
        private int kernelInit;
        private int kernelCalculateAabb;
        private int kernelCalculateMortonCodes;
        private int kernelInitClusters;
        private int kernelMergeClusters;
        private int kernelFindPreferredNeighbor;
        private int kernelWriteLeafNodes;
        private int kernelBuildTreeBottomUp;
        private int kernelClearUpdateFlags;

        private ComputeShader shaderReorderTriangleIndices;
        private int kernelOrderIndices;
        private int kernelCopyOrderedIndicesBack;

        private RadixSort radixSort;
        private Scan scan;

        const uint kTrianglesPerThread = 8u;
        const uint kGroupSize = 256u;
        const uint kTrianglesPerGroup = kTrianglesPerThread * kGroupSize;

        public HlbvhBuilder(RadeonRaysShaders shaders)
        {
            shaderBuildHlbvh = shaders.buildHlbvh;
            kernelInit = shaderBuildHlbvh.FindKernel("Init");
            kernelCalculateAabb = shaderBuildHlbvh.FindKernel("CalculateAabb");
            kernelCalculateMortonCodes = shaderBuildHlbvh.FindKernel("CalculateMortonCodes");
            kernelWriteLeafNodes = shaderBuildHlbvh.FindKernel("WriteLeafNodes");
            kernelBuildTreeBottomUp = shaderBuildHlbvh.FindKernel("BuildTreeBottomUp");

            kernelInitClusters = shaderBuildHlbvh.FindKernel("InitClusters");
            kernelFindPreferredNeighbor = shaderBuildHlbvh.FindKernel("FindPreferredNeighbor");
            kernelMergeClusters = shaderBuildHlbvh.FindKernel("MergeClusters");
            kernelClearUpdateFlags = shaderBuildHlbvh.FindKernel("ClearUpdateFlags");

            shaderReorderTriangleIndices = shaders.reorderTriangleIndices;
            kernelOrderIndices = shaderReorderTriangleIndices.FindKernel("OrderIndices");
            kernelCopyOrderedIndicesBack = shaderReorderTriangleIndices.FindKernel("CopyOrderedIndicesBack");

            radixSort = new RadixSort(shaders);
            scan = new Scan(shaders);
        }

        public uint GetScratchDataSizeInDwords(uint triangleCount)
        {
            var scratchLayout = GetScratchBufferLayout(triangleCount);
            return scratchLayout.TotalSize;
        }

        public static uint GetBvhNodeCount(uint leafCount)
        {
            return 2 * leafCount - 1;
        }

        public static uint GetBvhNodeCountPrediction(uint leafCount)
        {
            return (uint)((double)leafCount*0.8) + 10;
        }

        public uint GetResultDataSizeInDwords(uint triangleCount)
        {
            var bvhNodeCount = GetBvhNodeCount(triangleCount) + 1; // plus one for header
            uint sizeOfNode = 16;
            return bvhNodeCount * sizeOfNode;
        }

        public uint GetResultDataSizeInDwordsPrediction(uint triangleCount)
        {
            var bvhNodeCount = GetBvhNodeCountPrediction(triangleCount) + 1; // plus one for header
            uint sizeOfNode = 16;
            return bvhNodeCount * sizeOfNode;
        }

        struct ScratchBufferOffsets
        {
            public uint Aabb;
            public uint SortedPrimitiveRefs;
            public uint SortedMortonCodes;

            // Overlaps with TempBvh
            public uint PrimitiveRefs;
            public uint MortonCodes;
            public uint SortMemory;

            // Overlaps with PrimitiveRefs
            public uint TempBvh;
            public uint EnabledNodes;
            public uint ScanScratch;

            public uint ClusterValidity;
            public uint ClusterRange;
            public uint PreferredNeighbor;
            public uint ClusterToNodeIndex;
            public uint Deltas;
            public uint InternalNodeRange;

            public uint TotalSize;
        }

        public void Execute(
            CommandBuffer cmd,
            GraphicsBuffer vertices, int verticesOffset, uint vertexStride,
            GraphicsBuffer indices, int indicesOffset, uint triangleCount,
            GraphicsBuffer scratch, GraphicsBuffer result, uint resultOffset, uint resultSizeInNodes,
            uint reduceMemoryIterations = 2)
        {
            Common.EnableKeyword(cmd, shaderBuildHlbvh, "TOP_LEVEL", false);
            Common.EnableKeyword(cmd, shaderBuildHlbvh, "NO_REDUCTION", reduceMemoryIterations == 0);
            var scratchLayout = GetScratchBufferLayout(triangleCount);

            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_indices_offset, indicesOffset);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_vertices_offset, verticesOffset);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_constants_vertex_stride, (int)vertexStride);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_constants_triangle_count, (int)triangleCount);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_bvh_offset, (int)resultOffset);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_bvh_max_node_count, (int)resultSizeInNodes-1);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_cluster_validity_offset, (int)scratchLayout.ClusterValidity);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_cluster_range_offset, (int)scratchLayout.ClusterRange);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_neighbor_offset, (int)scratchLayout.PreferredNeighbor);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_cluster_to_node_offset, (int)scratchLayout.ClusterToNodeIndex);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_deltas_offset, (int)scratchLayout.Deltas);
            cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_internal_node_range_offset, (int)scratchLayout.InternalNodeRange);

            BindKernelArguments(cmd, kernelInit, vertices, indices, scratch, scratchLayout, result, false);
            cmd.DispatchCompute(shaderBuildHlbvh, kernelInit, 1, 1, 1);

            BindKernelArguments(cmd, kernelCalculateAabb, vertices, indices, scratch, scratchLayout, result, false);
            cmd.DispatchCompute(shaderBuildHlbvh, kernelCalculateAabb, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);

            BindKernelArguments(cmd, kernelCalculateMortonCodes, vertices, indices, scratch, scratchLayout, result, false);
            cmd.DispatchCompute(shaderBuildHlbvh, kernelCalculateMortonCodes, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);

            radixSort.Execute(cmd, scratch,
                scratchLayout.MortonCodes, scratchLayout.SortedMortonCodes,
                scratchLayout.PrimitiveRefs, scratchLayout.SortedPrimitiveRefs,
                scratchLayout.SortMemory, triangleCount);

            // Leaf nodes only store a range of triangle indices and not the triangle indices themselves. (FirstTriangleIndex, TriangleCount).
            // That requires to sort the input index buffer in morton code order.
            ReorderIndexBuffer(cmd, indices, indicesOffset, triangleCount, scratch, scratchLayout);

            if (reduceMemoryIterations != 0)
            {
                // Original RadeonRays impl stores only one triangle per leaf noe
                // Added optional path that starts by agglomerating multiple triangles per node before starting the BVH tree construction.
                // Based on PLOC paper ("Parallel Locally-Ordered Clustering for Bounding Volume Hierarchy Construction")
                BindKernelArguments(cmd, kernelInitClusters, vertices, indices, scratch, scratchLayout, result, true);
                cmd.DispatchCompute(shaderBuildHlbvh, kernelInitClusters, (int)Common.CeilDivide(triangleCount, kGroupSize), 1, 1);

                for (int i = 0; i < reduceMemoryIterations; ++i)
                {
                    BindKernelArguments(cmd, kernelFindPreferredNeighbor, vertices, indices, scratch, scratchLayout, result, true);
                    cmd.DispatchCompute(shaderBuildHlbvh, kernelFindPreferredNeighbor, (int)Common.CeilDivide(triangleCount, kGroupSize), 1, 1);

                    BindKernelArguments(cmd, kernelMergeClusters, vertices, indices, scratch, scratchLayout, result, true);
                    cmd.DispatchCompute(shaderBuildHlbvh, kernelMergeClusters, (int)Common.CeilDivide(triangleCount, kGroupSize), 1, 1);
                }

                scan.Execute(cmd, scratch, scratchLayout.ClusterValidity, scratchLayout.ClusterToNodeIndex, scratchLayout.ScanScratch, triangleCount);

                BindKernelArguments(cmd, kernelWriteLeafNodes, vertices, indices, scratch, scratchLayout, result, true);
                cmd.DispatchCompute(shaderBuildHlbvh, kernelWriteLeafNodes, (int)Common.CeilDivide(triangleCount, kGroupSize), 1, 1);
            }
            else
            {
                BindKernelArguments(cmd, kernelClearUpdateFlags, vertices, indices, scratch, scratchLayout, result, true);
                cmd.DispatchCompute(shaderBuildHlbvh, kernelClearUpdateFlags, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);
            }

            // In RadeonRays, HLBVH construction was based on "Maximizing Parallelism in the Construction of BVHs, Octrees, and k-d Trees" paper
            // Replaced by impl by "Fast and Simple Agglomerative LBVH Construction" paper that does everything in a single bottom-up pass.
            BindKernelArguments(cmd, kernelBuildTreeBottomUp, vertices, indices, scratch, scratchLayout, result, true);
            cmd.DispatchCompute(shaderBuildHlbvh, kernelBuildTreeBottomUp, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);
        }

        private ScratchBufferOffsets cachedScratchOffsets;
        private uint cachedTriangleCount = 0;

        ScratchBufferOffsets GetScratchBufferLayout(uint triangleCount)
        {
            if (cachedTriangleCount == triangleCount)
            {
                return cachedScratchOffsets;
            }

            var result = new ScratchBufferOffsets();

            uint offset = 0;
            result.Aabb = offset;
            offset += 6;
            result.SortedPrimitiveRefs = offset;
            offset += triangleCount;
            result.SortedMortonCodes = offset;
            offset += triangleCount;

            result.PrimitiveRefs = offset;
            offset += triangleCount;
            result.MortonCodes = offset;
            offset += triangleCount;
            result.SortMemory = offset;
            offset += (uint)radixSort.GetScratchDataSizeInDwords(triangleCount);
            result.TotalSize = offset;

            // used by kernelWriteLeafNodes
            result.ClusterValidity = result.PrimitiveRefs;
            result.ClusterRange = result.PrimitiveRefs + triangleCount;
            result.ClusterToNodeIndex = result.PrimitiveRefs + 2*triangleCount;
            result.Deltas = result.PrimitiveRefs + 3*triangleCount;

            result.ScanScratch = result.Deltas;

            // used by Clustering
            result.PreferredNeighbor = result.ClusterToNodeIndex;

            // used by kernelBuildTreeBottomUp
            result.InternalNodeRange = result.ClusterValidity;

            result.TotalSize = math.max(result.TotalSize, result.Deltas+triangleCount);

            cachedScratchOffsets = result;
            cachedTriangleCount = triangleCount;

            return result;
        }

        private void BindKernelArguments(
            CommandBuffer cmd,
            int kernel,
            GraphicsBuffer vertices,
            GraphicsBuffer indices,
            GraphicsBuffer scratch,
            ScratchBufferOffsets scratchLayout,
            GraphicsBuffer result,
            bool setSortedCodes)
        {
            cmd.SetComputeBufferParam(shaderBuildHlbvh, kernel, SID.g_vertices, vertices);
            cmd.SetComputeBufferParam(shaderBuildHlbvh, kernel, SID.g_indices, indices);
            cmd.SetComputeBufferParam(shaderBuildHlbvh, kernel, SID.g_scratch_buffer, scratch);
            cmd.SetComputeBufferParam(shaderBuildHlbvh, kernel, SID.g_bvh, result);

            if (setSortedCodes)
            {
                cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_morton_codes_offset, (int)scratchLayout.SortedMortonCodes);
                cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_primitive_refs_offset, (int)scratchLayout.SortedPrimitiveRefs);
            }
            else
            {
                cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_morton_codes_offset, (int)scratchLayout.MortonCodes);
                cmd.SetComputeIntParam(shaderBuildHlbvh, SID.g_primitive_refs_offset, (int)scratchLayout.PrimitiveRefs);
            }
        }
        private void ReorderIndexBuffer(
            CommandBuffer cmd,
            GraphicsBuffer indices, int indicesOffset, uint triangleCount,
            GraphicsBuffer scratch, ScratchBufferOffsets scratchLayout)
        {
            cmd.SetComputeIntParam(shaderReorderTriangleIndices, SID.g_indices_offset, indicesOffset);
            cmd.SetComputeIntParam(shaderReorderTriangleIndices, SID.g_constants_triangle_count, (int)triangleCount);
            cmd.SetComputeIntParam(shaderReorderTriangleIndices, SID.g_sorted_prim_refs_offset, (int)scratchLayout.SortedPrimitiveRefs);
            cmd.SetComputeIntParam(shaderReorderTriangleIndices, SID.g_temp_indices_offset, (int)scratchLayout.PrimitiveRefs);

            cmd.SetComputeBufferParam(shaderReorderTriangleIndices, kernelOrderIndices, SID.g_indices, indices);
            cmd.SetComputeBufferParam(shaderReorderTriangleIndices, kernelOrderIndices, SID.g_scratch_buffer, scratch);
            cmd.DispatchCompute(shaderReorderTriangleIndices, kernelOrderIndices, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);

            cmd.SetComputeBufferParam(shaderReorderTriangleIndices, kernelCopyOrderedIndicesBack, SID.g_indices, indices);
            cmd.SetComputeBufferParam(shaderReorderTriangleIndices, kernelCopyOrderedIndicesBack, SID.g_scratch_buffer, scratch);
            cmd.DispatchCompute(shaderReorderTriangleIndices, kernelCopyOrderedIndicesBack, (int)Common.CeilDivide(triangleCount, kTrianglesPerGroup), 1, 1);
        }
    }
}
