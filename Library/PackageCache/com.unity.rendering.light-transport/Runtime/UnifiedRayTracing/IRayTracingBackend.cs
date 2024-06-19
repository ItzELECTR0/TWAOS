using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal interface IRayTracingBackend
    {
        IRayTracingShader CreateRayTracingShader(
            Object shader,
            string kernelName);

        IRayTracingAccelStruct CreateAccelerationStructure(
            AccelerationStructureOptions options,
            GeometryPool geometryPool,
            ReferenceCounter counter);
    }
}
