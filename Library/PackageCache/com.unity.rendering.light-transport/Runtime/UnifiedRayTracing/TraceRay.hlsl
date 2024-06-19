#ifndef UNIFIEDRAYTRACING_TRACERAY_HLSL
#define UNIFIEDRAYTRACING_TRACERAY_HLSL

#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Bindings.hlsl"

namespace UnifiedRT {

static const uint kRayFlagNone = 0x0;
static const uint kRayFlagCullBackFacingTriangles = 0x10;
static const uint kRayFlagCullFrontFacingTriangles = 0x20;

#if defined(RAYTRACING_BACKEND_HARDWARE)

Hit TraceRayClosestHit(DispatchInfo dispatchInfo, RayTracingAccelStruct accelStruct, uint instanceMask, Ray ray, uint rayFlags)
{
    RayDesc rayDesc;
    rayDesc.Origin = ray.origin;
    rayDesc.TMin = ray.tMin;
    rayDesc.Direction = ray.direction;
    rayDesc.TMax = ray.tMax;

    Hit payload;
	TraceRay(accelStruct.accelStruct, RAY_FLAG_FORCE_OPAQUE | rayFlags, instanceMask, 0, 1, 0, rayDesc, payload);

    return payload;
}

bool TraceRayAnyHit(DispatchInfo dispatchInfo, RayTracingAccelStruct accelStruct, uint instanceMask, Ray ray, uint rayFlags)
{
    RayDesc rayDesc;
    rayDesc.Origin = ray.origin;
    rayDesc.TMin = ray.tMin;
    rayDesc.Direction = ray.direction;
    rayDesc.TMax = ray.tMax;

    Hit payLoadShadow = (Hit)0;
    TraceRay(accelStruct.accelStruct, RAY_FLAG_SKIP_CLOSEST_HIT_SHADER | RAY_FLAG_FORCE_OPAQUE | RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | rayFlags, instanceMask, 0, 1, 0, rayDesc, payLoadShadow);

    return payLoadShadow.IsValid();
}

#elif defined(RAYTRACING_BACKEND_COMPUTE)

int GetCullMode(uint rayFlags)
{
    int cullMode = CULL_MODE_NONE;

    if (rayFlags & kRayFlagCullFrontFacingTriangles)
        cullMode = CULL_MODE_FRONTFACE;

    if (rayFlags & kRayFlagCullBackFacingTriangles)
        cullMode = CULL_MODE_BACKFACE;

    return cullMode;
}

Hit TraceRayClosestHit(DispatchInfo dispatchInfo, RayTracingAccelStruct accelStruct, uint instanceMask, Ray ray, uint rayFlags)
{
    TraceParams traceParams;
    traceParams.bvh = accelStruct.bvh;
    traceParams.bottom_bvhs = accelStruct.bottom_bvhs;
    traceParams.stack = g_stack;
    traceParams.instance_infos = accelStruct.instance_infos;
    traceParams.globalThreadIndex = dispatchInfo.globalThreadIndex;
    traceParams.localThreadIndex = dispatchInfo.localThreadIndex;

    VertexPoolDesc vertex_pool_desc;
    vertex_pool_desc.index_buffer = g_globalIndexBuffer;
    vertex_pool_desc.vertex_buffer = g_globalVertexBuffer;
    vertex_pool_desc.vertex_stride = g_globalVertexBufferStride;

    int cull_mode = GetCullMode(rayFlags);

    TraceHitResult hitData = TraceRay(traceParams, vertex_pool_desc, ray.origin, ray.tMin, ray.direction, ray.tMax, instanceMask, cull_mode, true);

    Hit res;
    res.instanceIndex = hitData.inst_id != -1 ? GetUserInstanceID(traceParams, hitData.inst_id) : -1;
    res.primitiveIndex = hitData.prim_id;
    res.uvBarycentrics = hitData.uv;
    res.hitDistance = hitData.hit_distance;
    res.isFrontFace = hitData.front_face;

    return res;
}

bool TraceRayAnyHit(DispatchInfo dispatchInfo, RayTracingAccelStruct accelStruct, uint instanceMask, Ray ray, uint rayFlags)
{
    TraceParams traceParams;
    traceParams.bvh = accelStruct.bvh;
    traceParams.bottom_bvhs = accelStruct.bottom_bvhs;
    traceParams.stack = g_stack;
    traceParams.instance_infos = accelStruct.instance_infos;
    traceParams.globalThreadIndex = dispatchInfo.globalThreadIndex;
    traceParams.localThreadIndex = dispatchInfo.localThreadIndex;

    VertexPoolDesc vertex_pool_desc;
    vertex_pool_desc.index_buffer = g_globalIndexBuffer;
    vertex_pool_desc.vertex_buffer = g_globalVertexBuffer;
    vertex_pool_desc.vertex_stride = g_globalVertexBufferStride;

    int cull_mode = GetCullMode(rayFlags);

    TraceHitResult hit = TraceRay(traceParams, vertex_pool_desc, ray.origin, ray.tMin, ray.direction, ray.tMax, instanceMask, cull_mode, false);

    return hit.inst_id != INVALID_NODE;
}

#endif

} // namespace UnifiedRT

#endif // UNIFIEDRAYTRACING_TRACERAY_HLSL
