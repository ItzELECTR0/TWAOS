using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
    internal class RestructureBvh : IDisposable
    {
        private ComputeShader shader;
        private int kernelInitPrimitiveCounts;
        private int kernelFindTreeletRoots;
        private int kernelRestructure;
        private int kernelPrepareTreeletsDispatchSize;
        private int numIterations = 3;
        public GraphicsBuffer treeletDispatchIndirectBuffer;

        const uint kGroupSize = 256u;
        const uint kTrianglesPerThread = 8u;
        const uint kTrianglesPerGroup = kTrianglesPerThread * kGroupSize;
        const uint kMinPrimitivesPerTreelet = 64u;
        private const int kMaxThreadGroupsPerDispatch = 65535;

        public RestructureBvh(RadeonRaysShaders shaders)
        {
            shader = shaders.restructureBvh;
            kernelInitPrimitiveCounts = shader.FindKernel("InitPrimitiveCounts");
            kernelFindTreeletRoots = shader.FindKernel("FindTreeletRoots");
            kernelRestructure = shader.FindKernel("Restructure");
            kernelPrepareTreeletsDispatchSize = shader.FindKernel("PrepareTreeletsDispatchSize");

            treeletDispatchIndirectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 6, sizeof(uint));
        }
        public void Dispose()
        {
            treeletDispatchIndirectBuffer.Dispose();
        }

        public ulong GetScratchDataSizeInDwords(uint triangleCount)
        {
            var scratchLayout = GetScratchBufferLayout(triangleCount);
            return scratchLayout.TotalSize;
        }

        public static uint GetBvhNodeCount(uint leafCount)
        {
            return 2 * leafCount - 1;
        }

        public void Execute(
            CommandBuffer cmd,
            GraphicsBuffer vertices, int verticesOffset, uint vertexStride,
            GraphicsBuffer indices, int indicesOffset, uint triangleCount,
            GraphicsBuffer scratch, GraphicsBuffer result, uint resultOffset, string objName = null)
        {
            var scratchLayout = GetScratchBufferLayout(triangleCount);
            Common.EnableKeyword(cmd, shader, "TOP_LEVEL", false);

            cmd.SetComputeIntParam(shader, SID.g_indices_offset, indicesOffset);
            cmd.SetComputeIntParam(shader, SID.g_vertices_offset, verticesOffset);
            cmd.SetComputeIntParam(shader, SID.g_constants_vertex_stride, (int)vertexStride);
            cmd.SetComputeIntParam(shader, SID.g_constants_triangle_count, (int)triangleCount);
            cmd.SetComputeIntParam(shader, SID.g_treelet_count_offset, (int)scratchLayout.TreeletCount);
            cmd.SetComputeIntParam(shader, SID.g_treelet_roots_offset, (int)scratchLayout.TreeletRoots);
            cmd.SetComputeIntParam(shader, SID.g_primitive_counts_offset, (int)scratchLayout.PrimitiveCounts);
            cmd.SetComputeIntParam(shader, SID.g_bvh_offset, (int)resultOffset);

            uint minPrimitivePerTreelet = kMinPrimitivesPerTreelet;
            for (int i = 0; i < numIterations; ++i)
            {
                cmd.SetComputeIntParam(shader, SID.g_constants_min_prims_per_treelet, (int)minPrimitivePerTreelet);

                BindKernelArguments(cmd, kernelInitPrimitiveCounts, vertices, indices, scratch, result);
                cmd.DispatchCompute(shader, kernelInitPrimitiveCounts, (int)Common.CeilDivide(kTrianglesPerGroup, kGroupSize), 1, 1);

                BindKernelArguments(cmd, kernelFindTreeletRoots, vertices, indices, scratch, result);
                cmd.DispatchCompute(shader, kernelFindTreeletRoots, (int)Common.CeilDivide(kTrianglesPerGroup, kGroupSize), 1, 1);

                BindKernelArguments(cmd, kernelPrepareTreeletsDispatchSize, vertices, indices, scratch, result);
                cmd.DispatchCompute(shader, kernelPrepareTreeletsDispatchSize, 1, 1, 1);

                BindKernelArguments(cmd, kernelRestructure, vertices, indices, scratch, result);
                cmd.SetComputeIntParam(shader, SID.g_remainder_treelets, 0);
                cmd.DispatchCompute(shader, kernelRestructure, treeletDispatchIndirectBuffer, 0);

                if (Common.CeilDivide(triangleCount, minPrimitivePerTreelet) > kMaxThreadGroupsPerDispatch)
                {
                    cmd.SetComputeIntParam(shader, SID.g_remainder_treelets, 1);
                    cmd.DispatchCompute(shader, kernelRestructure, treeletDispatchIndirectBuffer, 3 * sizeof(uint));
                }

                minPrimitivePerTreelet *= 2;
            }
        }

        private void BindKernelArguments(
            CommandBuffer cmd, int kernel,
            GraphicsBuffer vertices, GraphicsBuffer indices,
            GraphicsBuffer scratch, GraphicsBuffer result)
        {
            cmd.SetComputeBufferParam(shader, kernel, SID.g_vertices, vertices);
            cmd.SetComputeBufferParam(shader, kernel, SID.g_indices, indices);
            cmd.SetComputeBufferParam(shader, kernel, SID.g_scratch_buffer, scratch);
            cmd.SetComputeBufferParam(shader, kernel, SID.g_bvh, result);
            cmd.SetComputeBufferParam(shader, kernel, SID.g_treelet_dispatch_buffer, treeletDispatchIndirectBuffer);
        }

        struct ScratchBufferOffsets
        {
            public uint TreeletCount;
            public uint TreeletRoots;
            public uint PrimitiveCounts;
            public uint TotalSize;
        }

        ScratchBufferOffsets GetScratchBufferLayout(uint triangleCount)
        {
            var result = new ScratchBufferOffsets();

            uint offset = 0;
            result.TreeletCount = offset;
            offset += 1;
            result.TreeletRoots = offset;
            offset += triangleCount;
            result.PrimitiveCounts = offset;
            offset += GetBvhNodeCount(triangleCount);
            result.TotalSize = offset;

            return result;
        }
    }
}

