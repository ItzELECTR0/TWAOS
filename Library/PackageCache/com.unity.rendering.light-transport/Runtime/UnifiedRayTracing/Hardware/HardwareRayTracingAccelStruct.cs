
namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class HardwareRayTracingAccelStruct : IRayTracingAccelStruct
    {
        public RayTracingAccelerationStructure accelStruct { get; }

        internal AccelStructInstances instances { get { return m_Instances; } }

        Shader m_HWMaterialShader;
        Material m_RayTracingMaterial;
        AccelStructInstances m_Instances;

        internal HardwareRayTracingAccelStruct(GeometryPool geometryPool, Shader hwMaterialShader, ReferenceCounter counter)
        {
            m_HWMaterialShader = hwMaterialShader;
            LoadRayTracingMaterial();

            RayTracingAccelerationStructure.Settings settings = new RayTracingAccelerationStructure.Settings();
            settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;
            settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Manual;
            settings.layerMask = 255;

            accelStruct = new RayTracingAccelerationStructure(settings);
            m_Instances = new AccelStructInstances(geometryPool, counter);
        }

        public void Dispose()
        {
            m_Instances.Dispose();
            accelStruct?.Dispose();

            if (m_RayTracingMaterial != null)
                Utils.Destroy(m_RayTracingMaterial);
        }

        public int AddInstance(MeshInstanceDesc meshInstance)
        {
            LoadRayTracingMaterial();

            AccelStructInstances.InstanceEntry addedEntry;
            m_Instances.AddInstance(meshInstance, out addedEntry);

            int instanceIndex = addedEntry.indexInInstanceBuffer.block.offset;

            var instanceDesc = new RayTracingMeshInstanceConfig(addedEntry.mesh, (uint)meshInstance.subMeshIndex, m_RayTracingMaterial);
            instanceDesc.mask = meshInstance.mask;
            instanceDesc.enableTriangleCulling = meshInstance.enableTriangleCulling;
            instanceDesc.frontTriangleCounterClockwise = meshInstance.frontTriangleCounterClockwise;
            addedEntry.accelStructID = accelStruct.AddInstance(instanceDesc, meshInstance.localToWorldMatrix, null, (uint)instanceIndex);

            return instanceIndex;
        }

        public void RemoveInstance(int instanceHandle)
        {
            AccelStructInstances.InstanceEntry removedEntry;
            m_Instances.RemoveInstance(instanceHandle, out removedEntry);
            accelStruct.RemoveInstance(removedEntry.accelStructID);
        }

        public void ClearInstances()
        {
            m_Instances.ClearInstances();
            accelStruct.ClearInstances();
        }

        public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
        {
            AccelStructInstances.InstanceEntry entry;
            m_Instances.UpdateInstanceTransform(instanceHandle, localToWorldMatrix, out entry);
            accelStruct.UpdateInstanceTransform(entry.accelStructID, localToWorldMatrix);
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
            AccelStructInstances.InstanceEntry instanceEntry;
            m_Instances.UpdateInstanceMask(instanceHandle, mask, out instanceEntry);
            accelStruct.UpdateInstanceMask(instanceEntry.accelStructID, mask);
        }

        public void Build(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
        {
            cmd.BuildRayTracingAccelerationStructure(accelStruct);
        }

        public ulong GetBuildScratchBufferRequiredSizeInBytes()
        {
            // Unity's Hardware impl (RayTracingAccelerationStructure) does not require any scratchbuffers.
            // They are directly handled internally by the GfxDevice.
            return 0;
        }

        public void NextFrame()
        {
            m_Instances.NextFrame();
        }

        public void BindGeometryBuffers(CommandBuffer cmd, string name, IRayTracingShader shader)
        {
            m_Instances.BindGeometryBuffers(cmd, name, shader);
        }

        private void LoadRayTracingMaterial()
        {
            if (m_RayTracingMaterial == null)
                m_RayTracingMaterial = new Material(m_HWMaterialShader);
        }
    }
}

