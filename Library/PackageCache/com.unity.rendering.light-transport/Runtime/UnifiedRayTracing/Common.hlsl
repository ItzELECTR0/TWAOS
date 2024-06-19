#ifndef _RAYTRACING_COMMON_H_
#define _RAYTRACING_COMMON_H_

#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Bindings.hlsl"

#define K_T_MAX                 400000
#define FLT_EPSILON             1.192092896e-07F
#ifndef FLT_MAX
#define FLT_MAX 3.402823e+38
#endif

float Max3(float3 val)
{
    return max(max(val.x, val.y), val.z);
}

// Adapted from RayTracing Gems, A Fast and Robust Method for Avoiding Self-Intersection
// - Dropped the exact +N ulp computation, instead use, N * epsilon
// - Use max of distance components instead of per component offset
// - Use less conservative factors for error estimation
float3 OffsetRayOrigin(float3 p, float3 n)
{
    float distanceToOrigin = Max3(abs(p));
    float3 offset = (distanceToOrigin < 1 / 32.0f) ? FLT_EPSILON * 64.0f : FLT_EPSILON * 64.0f * distanceToOrigin;

    return p + offset * n;
}

struct PTHitGeom
{
    float3 worldPosition;
    float3 lastWorldPosition;
    float3 worldNormal;
    float3 worldFaceNormal;
    float2 textureUV;

    void FixNormals(float3 rayDirection)
    {
        worldFaceNormal = dot(rayDirection, worldFaceNormal) >= 0 ? -worldFaceNormal : worldFaceNormal;
        worldNormal = dot(worldNormal, worldFaceNormal) < 0 ? -worldNormal : worldNormal;
    }

    float3 NextRayOrigin()
    {
        return OffsetRayOrigin(worldPosition, worldFaceNormal);
    }

    float3 NextTransmissionRayOrigin()
    {
        return OffsetRayOrigin(worldPosition, -worldFaceNormal);
    }
};

PTHitGeom GetHitGeomInfo(UnifiedRT::RayTracingAccelStruct accelStruct, UnifiedRT::InstanceData instanceInfo, UnifiedRT::Hit hit)
{
    UnifiedRT::HitGeomAttributes attributes = UnifiedRT::FetchHitGeomAttributes(accelStruct, hit);

    PTHitGeom res;
    res.worldPosition = mul(instanceInfo.localToWorld, float4(attributes.position, 1)).xyz;
    res.lastWorldPosition = mul(instanceInfo.previousLocalToWorld, float4(attributes.position, 1)).xyz;
    res.worldNormal = normalize(mul((float3x3)instanceInfo.localToWorldNormals, attributes.normal));
    res.worldFaceNormal = normalize(mul((float3x3)instanceInfo.localToWorldNormals, attributes.faceNormal));
    res.textureUV = (float2)attributes.uv0;

    return res;
}

#endif
