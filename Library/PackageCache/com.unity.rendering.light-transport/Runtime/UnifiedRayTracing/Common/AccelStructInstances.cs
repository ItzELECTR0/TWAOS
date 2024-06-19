using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class AccelStructInstances : IDisposable
    {
        internal AccelStructInstances(GeometryPool geometryPool, ReferenceCounter counter)
        {
            m_GeometryPool = geometryPool;
            m_Counter = counter;
            counter.Inc();
        }

        public void Dispose()
        {
            m_Counter.Dec();

            foreach (InstanceEntry instanceEntry in m_Instances.Values)
            {
                GeometryPoolHandle geomHandle = instanceEntry.geometryPoolHandle;
                m_GeometryPool.Unregister(geomHandle);
            }
            m_GeometryPool.SendGpuCommands();

            m_InstanceBuffer?.Dispose();
        }

        public PersistentGpuArray<RTInstance> instanceBuffer  { get => m_InstanceBuffer; }
        public IReadOnlyCollection<InstanceEntry> instances { get => m_Instances.Values; }

        public int AddInstance(MeshInstanceDesc meshInstance, out InstanceEntry instanceEntry)
        {

            if (meshInstance.mesh == null)
                throw new System.ArgumentException("targetRenderer.mesh is null");

            GeometryPoolHandle geometryHandle;
            if (!m_GeometryPool.Register(meshInstance.mesh, out geometryHandle))
                throw new System.InvalidOperationException("Failed to allocate geometry data for instance");
            m_GeometryPool.SendGpuCommands();

            var slotAllocation = m_InstanceBuffer.Add(
                new RTInstance
                {
                    localToWorld = meshInstance.localToWorldMatrix,
                    localToWorldNormals = NormalMatrix(meshInstance.localToWorldMatrix),
                    previousLocalToWorld = meshInstance.localToWorldMatrix,
                    userMaterialID = meshInstance.materialID,
                    instanceMask = meshInstance.mask,
                    userInstanceID = meshInstance.instanceID,
                    geometryIndex = (uint)(m_GeometryPool.GetEntryGeomAllocation(geometryHandle).meshChunkTableAlloc.block.offset + meshInstance.subMeshIndex)
                });


            var allocInfo = m_GeometryPool.GetEntryGeomAllocation(geometryHandle).meshChunks[meshInstance.subMeshIndex];

            instanceEntry = new InstanceEntry
            {
                mesh = meshInstance.mesh,
                subMeshIndex = meshInstance.subMeshIndex,
                geometryPoolHandle = geometryHandle,
                indexInInstanceBuffer = slotAllocation,
                instanceMask = meshInstance.mask,
                vertexOffset = (uint)(allocInfo.vertexAlloc.block.offset) * ((uint)GeometryPool.GetVertexByteSize() / 4),
                indexOffset = (uint)allocInfo.indexAlloc.block.offset,
                triangleCullingEnabled = meshInstance.enableTriangleCulling,
                invertTriangleCulling = meshInstance.frontTriangleCounterClockwise
            };
            m_Instances.Add(slotAllocation.block.offset, instanceEntry);

            return slotAllocation.block.offset;
        }

        public GeometryPool.MeshChunk GetEntryGeomAllocation(GeometryPoolHandle handle, int submeshIndex)
        {
            return m_GeometryPool.GetEntryGeomAllocation(handle).meshChunks[submeshIndex];
        }

        public GraphicsBuffer indexBuffer { get { return m_GeometryPool.globalIndexBuffer; } }
        public GraphicsBuffer vertexBuffer { get { return m_GeometryPool.globalVertexBuffer; } }

        public void RemoveInstance(int instanceHandle, out InstanceEntry removedEntry)
        {
            bool success = m_Instances.TryGetValue(instanceHandle, out removedEntry);
            Assert.IsTrue(success);

            m_Instances.Remove(instanceHandle);
            m_InstanceBuffer.Remove(removedEntry.indexInInstanceBuffer);

            var geomHandle = removedEntry.geometryPoolHandle;
            m_GeometryPool.Unregister(geomHandle);
            m_GeometryPool.SendGpuCommands();
        }

        public void ClearInstances()
        {
            foreach (InstanceEntry instanceEntry in m_Instances.Values)
            {
                GeometryPoolHandle geomHandle = instanceEntry.geometryPoolHandle;
                m_GeometryPool.Unregister(geomHandle);
            }
            m_GeometryPool.SendGpuCommands();

            m_Instances.Clear();
            m_InstanceBuffer.Clear();
        }

        public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix, out InstanceEntry instanceEntry)
        {
            bool success = m_Instances.TryGetValue(instanceHandle, out instanceEntry);
            Assert.IsTrue(success);

            var instanceInfo = m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
            instanceInfo.localToWorld = localToWorldMatrix;
            instanceInfo.localToWorldNormals = NormalMatrix(localToWorldMatrix);
            m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, instanceInfo);

            m_TransformTouchedLastTimestamp = m_FrameTimestamp;
        }

        public void UpdateInstanceMaterialID(int instanceHandle, uint materialID)
        {
            InstanceEntry instanceEntry;
            bool success = m_Instances.TryGetValue(instanceHandle, out instanceEntry);
            Assert.IsTrue(success);

            var instanceInfo = m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
            instanceInfo.userMaterialID = materialID;
            m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, instanceInfo);
        }

        public void UpdateInstanceID(int instanceHandle, uint instanceID)
        {
            InstanceEntry instanceEntry;
            bool success = m_Instances.TryGetValue(instanceHandle, out instanceEntry);
            Assert.IsTrue(success);

            var instanceInfo = m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
            instanceInfo.userInstanceID = instanceID;
            m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, instanceInfo);
        }

        public void UpdateInstanceMask(int instanceHandle, uint mask, out InstanceEntry instanceEntry)
        {
            bool success = m_Instances.TryGetValue(instanceHandle, out instanceEntry);
            Assert.IsTrue(success);

            instanceEntry.instanceMask = mask;

            var instanceInfo = m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
            instanceInfo.instanceMask = mask;
            m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, instanceInfo);
        }

        public void NextFrame()
        {
            if ((m_FrameTimestamp - m_TransformTouchedLastTimestamp) <= 1)
            {
                m_InstanceBuffer.ModifyForEach(
                instance =>
                {
                    instance.previousLocalToWorld = instance.localToWorld;
                    return instance;
                });
            }

            m_FrameTimestamp++;
        }

        public bool instanceListValid => m_InstanceBuffer != null;

        public void BindGeometryBuffers(CommandBuffer cmd, string name, IRayTracingShader shader)
        {
            var gpuBuffer = m_InstanceBuffer.GetGpuBuffer(cmd);
            shader.SetBufferParam(cmd, Shader.PropertyToID(name + "instanceList"), gpuBuffer);

            // Geometry pool
            shader.SetBufferParam(cmd, Shader.PropertyToID("g_globalIndexBuffer"), m_GeometryPool.globalIndexBuffer);
            shader.SetBufferParam(cmd, Shader.PropertyToID("g_globalVertexBuffer"), m_GeometryPool.globalVertexBuffer);
            shader.SetIntParam(cmd, Shader.PropertyToID("g_globalVertexBufferStride"), m_GeometryPool.globalVertexBufferStrideBytes/4);
            shader.SetIntParam(cmd, Shader.PropertyToID("g_globalVertexBufferSize"), m_GeometryPool.verticesCount);
            shader.SetBufferParam(cmd, Shader.PropertyToID("g_MeshList"), m_GeometryPool.globalMeshChunkTableEntryBuffer);
        }

        public int GetInstanceCount()
        {
            return m_Instances.Count;
        }

        static private float4x4 NormalMatrix(float4x4 m)
        {
            float3x3 t = new float3x3(m);
            return new float4x4(math.inverse(math.transpose(t)), new float3(0.0));
        }

        GeometryPool m_GeometryPool;
        ReferenceCounter m_Counter;
        PersistentGpuArray<RTInstance> m_InstanceBuffer = new PersistentGpuArray<RTInstance>(100);

        public struct RTInstance
        {
            public float4x4 localToWorld;
            public float4x4 previousLocalToWorld;
            public float4x4 localToWorldNormals;
            public uint userInstanceID;
            public uint instanceMask;
            public uint userMaterialID;
            public uint geometryIndex;
        };

        public class InstanceEntry
        {
            public Mesh mesh; // need to keep reference, otherwise could be garbage collected and removed from HardwareRayTracingAccelStruct
            public int subMeshIndex;
            public GeometryPoolHandle geometryPoolHandle;
            public BlockAllocator.Allocation indexInInstanceBuffer;
            public int accelStructID;
            public uint instanceMask;
            public uint vertexOffset;
            public uint indexOffset;
            public bool triangleCullingEnabled;
            public bool invertTriangleCulling;
        }

        Dictionary<int, InstanceEntry> m_Instances = new Dictionary<int, InstanceEntry>();
        uint m_FrameTimestamp = 0;
        uint m_TransformTouchedLastTimestamp = 0;
    }
}
