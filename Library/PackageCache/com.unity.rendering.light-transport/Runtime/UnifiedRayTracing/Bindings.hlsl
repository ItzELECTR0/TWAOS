#ifndef UNIFIEDRAYTRACING_BINDINGS_HLSL
#define UNIFIEDRAYTRACING_BINDINGS_HLSL

#if defined(RAYTRACING_BACKEND_COMPUTE)
#define GROUP_SIZE RAYTRACING_GROUP_SIZE
#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/trace_ray.hlsl"
#endif

#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Common/GeometryPool/GeometryPoolDefs.cs.hlsl"
#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Common/GeometryPool/GeometryPool.hlsl"

namespace UnifiedRT {

struct Ray
{
    float3 origin;
    float  tMin;
    float3 direction;
    float  tMax;
};

struct Hit
{
    uint instanceIndex;
    uint primitiveIndex;
    float2 uvBarycentrics;
    float hitDistance;
    bool isFrontFace;

    bool IsValid()
    {
        return instanceIndex != -1;
    }
};

struct InstanceData
{
    float4x4 localToWorld;
    float4x4 previousLocalToWorld;
    float4x4 localToWorldNormals;
    uint userInstanceID;
    uint instanceMask;
    uint userMaterialID;
    uint geometryIndex;
};

struct DispatchInfo
{
    uint3 dispatchThreadID;
    uint localThreadIndex;
    uint3 dispatchDimensionsInThreads;
    uint globalThreadIndex;
};

struct RayTracingAccelStruct
{
#if defined(RAYTRACING_BACKEND_HARDWARE)
    RaytracingAccelerationStructure accelStruct;
#elif defined(RAYTRACING_BACKEND_COMPUTE)
    StructuredBuffer<BvhNode> bvh;
    StructuredBuffer<BvhNode> bottom_bvhs;
    StructuredBuffer<InstanceInfo> instance_infos;
#else
    #pragma message("Error, you must define either RAYTRACING_BACKEND_HARDWARE or RAYTRACING_BACKEND_COMPUTE")
#endif
    StructuredBuffer<InstanceData> instanceList;
};

#if defined(RAYTRACING_BACKEND_HARDWARE)
RayTracingAccelStruct GetAccelStruct(RaytracingAccelerationStructure accelStruct, StructuredBuffer<InstanceData> instanceList)
{
    RayTracingAccelStruct res;
    res.accelStruct = accelStruct;
    res.instanceList = instanceList;
    return res;
}

#define UNITY_DECLARE_RT_ACCEL_STRUCT(name) RaytracingAccelerationStructure name##accelStruct; StructuredBuffer<UnifiedRT::InstanceData> name##instanceList
#define UNITY_GET_RT_ACCEL_STRUCT(name) UnifiedRT::GetAccelStruct(name##accelStruct, name##instanceList)

#elif defined(RAYTRACING_BACKEND_COMPUTE)
RayTracingAccelStruct GetAccelStruct(
    StructuredBuffer<BvhNode> bvh,
    StructuredBuffer<BvhNode> bottomBvhs,
    StructuredBuffer<InstanceInfo> instanceInfos,
    StructuredBuffer<InstanceData> instanceList)
{
    RayTracingAccelStruct res;
    res.bvh = bvh;
    res.bottom_bvhs = bottomBvhs;
    res.instance_infos = instanceInfos;
    res.instanceList = instanceList;
    return res;
}

#define UNITY_DECLARE_RT_ACCEL_STRUCT(name) StructuredBuffer<BvhNode> name##bvh; StructuredBuffer<BvhNode> name##bottomBvhs; StructuredBuffer<InstanceInfo> name##instanceInfos; StructuredBuffer<UnifiedRT::InstanceData> name##instanceList
#define UNITY_GET_RT_ACCEL_STRUCT(name) UnifiedRT::GetAccelStruct(name##bvh, name##bottomBvhs, name##instanceInfos, name##instanceList)

#endif

} // namespace UnifiedRT

#if defined(RAYTRACING_BACKEND_COMPUTE)
RWStructuredBuffer<uint> g_stack;
#endif

StructuredBuffer<uint>             g_globalIndexBuffer;
StructuredBuffer<uint>             g_globalVertexBuffer;
int                                g_globalVertexBufferSize;
int                                g_globalVertexBufferStride;
StructuredBuffer<GeoPoolMeshChunk> g_MeshList;

#endif // UNIFIEDRAYTRACING_BINDINGS_HLSL
