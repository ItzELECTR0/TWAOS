using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Rendering.RadeonRays;

namespace UnityEngine.Rendering.UnifiedRayTracing
{

    internal class ComputeRayTracingAccelStruct : IRayTracingAccelStruct
    {
        internal ComputeRayTracingAccelStruct(
            AccelerationStructureOptions options, GeometryPool geometryPool, RayTracingResources resources,
            ReferenceCounter counter, int blasBufferInitialSizeBytes = 64 * 1024 * 1024)
        {
            m_CopyShader = resources.copyBuffer;

            RadeonRaysShaders shaders = new RadeonRaysShaders();
            shaders.bitHistogram = resources.bitHistogram;
            shaders.blockReducePart = resources.blockReducePart;
            shaders.blockScan = resources.blockScan;
            shaders.buildHlbvh = resources.buildHlbvh;
            shaders.reorderTriangleIndices = resources.reorderTriangleIndices;
            shaders.restructureBvh = resources.restructureBvh;
            shaders.scatter = resources.scatter;
            shaders.topLevelIntersector = resources.topLevelIntersector;
            shaders.intersector = resources.intersector;
            m_RadeonRaysAPI = new RadeonRaysAPI(shaders);

            m_AccelStructBuildFlags = (RadeonRays.BuildFlags)options.buildFlags;

            m_Blases = new Dictionary<(GeometryPoolHandle geomHandle, int subMeshIndex), MeshBlas>();

            var blasNodeCount = blasBufferInitialSizeBytes / RadeonRaysAPI.BvhNodeSizeInBytes();
            m_BlasBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, blasNodeCount, RadeonRaysAPI.BvhNodeSizeInBytes());

            m_BlasAllocator = new BlockAllocator();
            m_BlasAllocator.Initialize(blasNodeCount);

            m_NotEnoughMemoryBlases = new List<NotEnoughMemoryBlas>();

            m_Instances = new AccelStructInstances(geometryPool, counter);
        }

        internal GraphicsBuffer topLevelBvhBuffer { get { return m_TopLevelAccelStruct?.topLevelBvh; } }
        internal GraphicsBuffer bottomLevelBvhBuffer { get { return m_TopLevelAccelStruct?.bottomLevelBvhs; } }
        internal GraphicsBuffer instanceInfoBuffer { get { return m_TopLevelAccelStruct?.instanceInfos; } }
        internal AccelStructInstances instances { get { return m_Instances; } }

        public void Dispose()
        {
            m_Instances.Dispose();
            m_RadeonRaysAPI.Dispose();
            m_BlasBuffer.Dispose();
            m_BlasAllocator.Dispose();
            m_TopLevelAccelStruct?.Dispose();
        }

        public int AddInstance(MeshInstanceDesc meshInstance)
        {
            m_Instances.AddInstance(meshInstance, out AccelStructInstances.InstanceEntry instanceEntry);

            var blas = GetOrAllocateMeshBlas(instanceEntry.geometryPoolHandle, meshInstance.mesh, meshInstance.subMeshIndex);
            blas.IncRef();

            FreeTopLevelAccelStruct();

            return instanceEntry.indexInInstanceBuffer.block.offset;
        }

        public void RemoveInstance(int instanceHandle)
        {
            m_Instances.RemoveInstance(instanceHandle, out AccelStructInstances.InstanceEntry instanceEntry);

            var geomHandle = instanceEntry.geometryPoolHandle;
            var subMeshIndex = instanceEntry.subMeshIndex;

            if (geomHandle.valid)
            {
                var meshBlas = m_Blases[(geomHandle, subMeshIndex)];
                meshBlas.DecRef();
                if (meshBlas.IsUnreferenced())
                    DeleteMeshBlas(geomHandle, subMeshIndex, meshBlas);
            }

            FreeTopLevelAccelStruct();
        }

        public void ClearInstances()
        {
            m_Instances.ClearInstances();

            m_Blases.Clear();
            var currentCapacity = m_BlasAllocator.capacity;
            m_BlasAllocator.Dispose();
            m_BlasAllocator = new BlockAllocator();
            m_BlasAllocator.Initialize(currentCapacity);

            FreeTopLevelAccelStruct();
        }

        public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
        {
            m_Instances.UpdateInstanceTransform(instanceHandle, localToWorldMatrix, out AccelStructInstances.InstanceEntry instanceEntry);
            FreeTopLevelAccelStruct();
        }

        public void UpdateInstanceMaterialID(int instanceHandle, uint materialID)
        {
            m_Instances.UpdateInstanceMaterialID(instanceHandle, materialID);
        }

        public void UpdateInstanceID(int instanceHandle, uint instanceID)
        {
            m_Instances.UpdateInstanceID(instanceHandle, instanceID);
        }

        public void UpdateInstanceMask(int instanceHandle, uint mask)
        {
            m_Instances.UpdateInstanceMask(instanceHandle, mask, out AccelStructInstances.InstanceEntry instanceEntry);
            FreeTopLevelAccelStruct();
        }

        public void Build(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            var requiredScratchSize = GetBuildScratchBufferRequiredSizeInBytes();
            if (requiredScratchSize > 0 && (scratchBuffer == null || ((ulong)(scratchBuffer.count * scratchBuffer.stride) < requiredScratchSize)))
            {
                throw new System.ArgumentException("scratchBuffer size is too small");
            }

            if (requiredScratchSize > 0 && scratchBuffer.stride != 4)
            {
                throw new System.ArgumentException("scratchBuffer stride must be 4");
            }

            bool blasesReallocated = ReallocateNotEnoughMemoryBlases(cmd, scratchBuffer);

            if (m_TopLevelAccelStruct != null && !blasesReallocated)
                return;

            CreateBvh(cmd, scratchBuffer);
        }

        public ulong GetBuildScratchBufferRequiredSizeInBytes()
        {
            return Math.Max(GetBvhBuildScratchBufferSizeInDwords() * 4, GetNotEnoughMemoryRebuildScratchBufferSizeInDwords()*4);
        }

        public void NextFrame()
        {
            m_Instances.NextFrame();
        }

        private void FreeTopLevelAccelStruct()
        {
            m_TopLevelAccelStruct?.Dispose();
            m_TopLevelAccelStruct = null;
        }

        private GraphicsBuffer LoadVertexAttribInfo(Mesh mesh, VertexAttribute attribute, out int attributeStride, out int attributeOffset)
        {
            if (!mesh.HasVertexAttribute(attribute))
            {
                attributeStride = attributeOffset = 0;
                return null;
            }

            int stream = mesh.GetVertexAttributeStream(attribute);
            attributeStride = mesh.GetVertexBufferStride(stream);
            attributeOffset = mesh.GetVertexAttributeOffset(attribute);

            return mesh.GetVertexBuffer(stream);
        }

        private MeshBlas GetOrAllocateMeshBlas(GeometryPoolHandle geometryHandle, Mesh mesh, int subMeshIndex)
        {
            MeshBlas blas;
            if (m_Blases.TryGetValue((geometryHandle, subMeshIndex), out blas))
                return blas;

            blas = new MeshBlas();
            AllocateBlas(geometryHandle, mesh, subMeshIndex, out blas.buildInfo, out blas.blasAllocation);

            m_Blases[(geometryHandle, subMeshIndex)] = blas;

            return blas;
        }
        void AllocateBlas(
            GeometryPoolHandle geometryHandle, Mesh mesh, int submeshIndex,
            out MeshBuildInfo buildInfo, out BlockAllocator.Allocation blasAllocation)
        {
            var vertexSizeInDwords = GeometryPool.GetVertexByteSize() / 4;
            var bvhNodeSizeInDwords = RadeonRaysAPI.BvhNodeSizeInDwords();

            var allocInfo = m_Instances.GetEntryGeomAllocation(geometryHandle, submeshIndex);
            int verticesStart = allocInfo.vertexAlloc.block.offset;
            uint vertexCount = (uint)(allocInfo.vertexAlloc.block.count);
            var indexStartOffset = allocInfo.indexAlloc.block.offset;
            uint indexCount = (uint)allocInfo.indexAlloc.block.count;

            var meshBuildInfo = new MeshBuildInfo();
            meshBuildInfo.vertices = m_Instances.vertexBuffer;
            meshBuildInfo.verticesStartOffset = verticesStart * vertexSizeInDwords;
            meshBuildInfo.triangleIndices = m_Instances.indexBuffer;
            meshBuildInfo.vertexCount = vertexCount;
            meshBuildInfo.triangleCount = indexCount / 3;
            meshBuildInfo.indicesStartOffset = indexStartOffset;
            meshBuildInfo.vertexStride = (uint)vertexSizeInDwords;
            buildInfo = meshBuildInfo;

            var requirements = m_RadeonRaysAPI.GetMeshBuildMemoryRequirements(meshBuildInfo, m_AccelStructBuildFlags);
            var allocationNodeCount = (int)(requirements.resultSizeInDwords / (ulong)bvhNodeSizeInDwords);
            blasAllocation = AllocateBlas(allocationNodeCount);
        }

        private void DeleteMeshBlas(GeometryPoolHandle geometryHandle, int submeshIndex, MeshBlas blas)
        {
            m_BlasAllocator.FreeAllocation(blas.blasAllocation);
            blas.blasAllocation = BlockAllocator.Allocation.Invalid;

            m_Blases.Remove((geometryHandle, submeshIndex));
        }

        private ulong GetBvhBuildScratchBufferSizeInDwords()
        {
            var bvhNodeSizeInDwords = RadeonRaysAPI.BvhNodeSizeInDwords();
            ulong scratchBufferSize = 0;

            foreach (var meshBlas in m_Blases)
            {
                if (meshBlas.Value.bvhBuilt)
                    continue;

                var requirements = m_RadeonRaysAPI.GetMeshBuildMemoryRequirements(meshBlas.Value.buildInfo, m_AccelStructBuildFlags);
                Assert.AreEqual(requirements.resultSizeInDwords / (ulong)bvhNodeSizeInDwords, (ulong)meshBlas.Value.blasAllocation.block.count);
                scratchBufferSize = math.max(scratchBufferSize, requirements.buildScratchSizeInDwords);
            }

            var topLevelScratchSize = m_RadeonRaysAPI.GetSceneBuildMemoryRequirements((uint)m_Instances.GetInstanceCount()).buildScratchSizeInDwords;
            scratchBufferSize = math.max(scratchBufferSize, topLevelScratchSize);
            scratchBufferSize = math.max(4, scratchBufferSize);

            return scratchBufferSize;
        }

        private ulong GetNotEnoughMemoryRebuildScratchBufferSizeInDwords()
        {
            ulong scratchBufferSize = 0;
            foreach (var blas in m_NotEnoughMemoryBlases)
            {
                if (!blas.blas.blasAllocation.valid)
                    continue;

                blas.blas.buildInfo.vertices = m_Instances.vertexBuffer;
                blas.blas.buildInfo.triangleIndices = m_Instances.indexBuffer;

                var requirements = m_RadeonRaysAPI.GetMeshBuildMemoryRequirements(blas.blas.buildInfo, m_AccelStructBuildFlags);
                scratchBufferSize = math.max(scratchBufferSize, requirements.buildScratchSizeInDwords);
            }

            return scratchBufferSize;
        }

        private void CreateBvh(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            BuildMissingBottomLevelAccelStructs(cmd, scratchBuffer);
            BuildTopLevelAccelStruct(cmd, scratchBuffer);
        }

        private void BuildMissingBottomLevelAccelStructs(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            foreach (var meshBlas in m_Blases.Values)
            {
                if (meshBlas.bvhBuilt)
                    continue;

                meshBlas.buildInfo.vertices = m_Instances.vertexBuffer;
                meshBlas.buildInfo.triangleIndices = m_Instances.indexBuffer;

                m_RadeonRaysAPI.BuildMeshAccelStruct(
                    cmd,
                    meshBlas.buildInfo, m_AccelStructBuildFlags,
                    scratchBuffer, m_BlasBuffer, (uint)meshBlas.blasAllocation.block.offset, (uint)meshBlas.blasAllocation.block.count,
                    req =>
                    {
                        var bvhHeader = req.GetData<uint>();
                        uint ok = bvhHeader[3];

                        if (ok == 0)
                        {
                            int count = (int)bvhHeader[0];
                            m_NotEnoughMemoryBlases.Add(new NotEnoughMemoryBlas{ blas = meshBlas, allocationNodeCount = count+1 });
                        }
                    });

                meshBlas.bvhBuilt = true;
            }

        }
        private void BuildTopLevelAccelStruct(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            var radeonRaysInstances = new RadeonRays.Instance[m_Instances.GetInstanceCount()];
            int i = 0;
            foreach (var instance in m_Instances.instances)
            {
                var blas = m_Blases[(instance.geometryPoolHandle, instance.subMeshIndex)];

                var meshAccelStructOffset = blas.blasAllocation.block.offset;
                radeonRaysInstances[i].meshAccelStructOffset = (uint)meshAccelStructOffset;
                radeonRaysInstances[i].localToWorldTransform = ConvertTranform(m_Instances.instanceBuffer.Get(instance.indexInInstanceBuffer).localToWorld);
                radeonRaysInstances[i].instanceMask = instance.instanceMask;
                radeonRaysInstances[i].vertexOffset = instance.vertexOffset;
                radeonRaysInstances[i].indexOffset = instance.indexOffset;
                radeonRaysInstances[i].triangleCullingEnabled = instance.triangleCullingEnabled;
                radeonRaysInstances[i].invertTriangleCulling = instance.invertTriangleCulling;
                radeonRaysInstances[i].userInstanceID = (uint)instance.indexInInstanceBuffer.block.offset;
                i++;
            }
            m_TopLevelAccelStruct?.Dispose();
            m_TopLevelAccelStruct = m_RadeonRaysAPI.BuildSceneAccelStruct(cmd, m_BlasBuffer, radeonRaysInstances, m_AccelStructBuildFlags, scratchBuffer);
        }

        public void BindGeometryBuffers(CommandBuffer cmd, string name, IRayTracingShader shader)
        {
            m_Instances.BindGeometryBuffers(cmd, name, shader);
        }


        static private RadeonRays.Transform ConvertTranform(Matrix4x4 input)
        {
            return new RadeonRays.Transform()
            {
                row0 = input.GetRow(0),
                row1 = input.GetRow(1),
                row2 = input.GetRow(2)
            };
        }

        private bool ReallocateNotEnoughMemoryBlases(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            bool HasBlasesToReallocate = m_NotEnoughMemoryBlases.Count != 0;
            if (!HasBlasesToReallocate)
                return false;

            foreach (var blas in m_NotEnoughMemoryBlases)
            {
                if (!blas.blas.blasAllocation.valid)
                    continue;

                m_BlasAllocator.FreeAllocation(blas.blas.blasAllocation);
                blas.blas.blasAllocation = AllocateBlas(blas.allocationNodeCount);

                m_RadeonRaysAPI.BuildMeshAccelStruct(
                    cmd,
                    blas.blas.buildInfo, m_AccelStructBuildFlags,
                    scratchBuffer, m_BlasBuffer, (uint)blas.blas.blasAllocation.block.offset, (uint)blas.blas.blasAllocation.block.count);
            }

            m_NotEnoughMemoryBlases.Clear();
            return true;
        }

        private BlockAllocator.Allocation AllocateBlas(int allocationNodeCount)
        {
            var allocation = m_BlasAllocator.Allocate(allocationNodeCount);
            if (!allocation.valid)
            {
                var oldBvhNodeCount = m_BlasAllocator.capacity;
                var newBvhNodeCount = m_BlasAllocator.Grow(m_BlasAllocator.capacity + allocationNodeCount);
                if ((ulong)newBvhNodeCount > ((ulong)2 * 1024 * 1024 * 1024) / (ulong)RadeonRaysAPI.BvhNodeSizeInBytes())
                    throw new System.OutOfMemoryException("Out of memory in BLAS buffer");

                var newBlasBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, newBvhNodeCount, RadeonRaysAPI.BvhNodeSizeInBytes());
                GraphicsHelpers.CopyBuffer(m_CopyShader, m_BlasBuffer, 0, newBlasBuffer, 0, oldBvhNodeCount * RadeonRaysAPI.BvhNodeSizeInDwords());

                m_BlasBuffer.Dispose();
                m_BlasBuffer = newBlasBuffer;
                allocation = m_BlasAllocator.Allocate(allocationNodeCount);
                Assertions.Assert.IsTrue(allocation.valid);
            }

            return allocation;
        }

        RadeonRaysAPI m_RadeonRaysAPI;
        RadeonRays.BuildFlags m_AccelStructBuildFlags = 0;

        Dictionary<(GeometryPoolHandle geomHandle, int subMeshIndex), MeshBlas> m_Blases;
        List<NotEnoughMemoryBlas> m_NotEnoughMemoryBlases;
        BlockAllocator m_BlasAllocator;
        GraphicsBuffer m_BlasBuffer;

        TopLevelAccelStruct? m_TopLevelAccelStruct = null;
        AccelStructInstances m_Instances;
        ComputeShader m_CopyShader;

        private class MeshBlas
        {
            public MeshBuildInfo buildInfo;
            public BlockAllocator.Allocation blasAllocation;
            public bool bvhBuilt = false;

            private uint refCount = 0;
            public void IncRef() { refCount++; }
            public void DecRef() { refCount--; }
            public bool IsUnreferenced() { return refCount == 0; }
        }

        struct NotEnoughMemoryBlas
        {
            public MeshBlas blas;
            public int allocationNodeCount;
        }
    }

}
