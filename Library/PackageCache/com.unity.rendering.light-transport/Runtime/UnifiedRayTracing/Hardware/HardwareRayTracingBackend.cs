namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class HardwareRayTracingBackend : IRayTracingBackend
    {
        public HardwareRayTracingBackend(RayTracingResources resources)
        {
            m_Resources = resources;
        }

        public IRayTracingShader CreateRayTracingShader(Object shader, string kernelName)
        {
            Debug.Assert(shader is RayTracingShader);
            return new HardwareRayTracingShader((RayTracingShader)shader, kernelName);
        }

        public IRayTracingAccelStruct CreateAccelerationStructure(AccelerationStructureOptions options, GeometryPool geometryPool, ReferenceCounter counter)
        {
            return new HardwareRayTracingAccelStruct(geometryPool, m_Resources.hardwareRayTracingMaterial, counter);
        }

        RayTracingResources m_Resources;
    }
}
