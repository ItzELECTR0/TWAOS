#ifndef UNIFIEDRAYTRACING_FETCH_GEOMETRY_HLSL
#define UNIFIEDRAYTRACING_FETCH_GEOMETRY_HLSL

#include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Bindings.hlsl"

#define INTERPOLATE_ATTRIBUTE(attr, barCoords) v.attr = v0.attr * (1.0 - barCoords.x - barCoords.y) + v1.attr * barCoords.x + v2.attr * barCoords.y

namespace UnifiedRT {

namespace Internal {
    GeoPoolVertex InterpolateVertices(GeoPoolVertex v0, GeoPoolVertex v1, GeoPoolVertex v2, float2 barycentricCoords)
    {
        GeoPoolVertex v;
        INTERPOLATE_ATTRIBUTE(pos, barycentricCoords);
        INTERPOLATE_ATTRIBUTE(N, barycentricCoords);
        INTERPOLATE_ATTRIBUTE(uv0, barycentricCoords);
        INTERPOLATE_ATTRIBUTE(uv1, barycentricCoords);
        return v;
    }

    uint3 FetchTriangleIndices(GeoPoolMeshChunk meshInfo, uint triangleID)
    {
        return uint3(
            g_globalIndexBuffer[meshInfo.indexOffset + 3 * triangleID],
            g_globalIndexBuffer[meshInfo.indexOffset + 3 * triangleID + 1],
            g_globalIndexBuffer[meshInfo.indexOffset + 3 * triangleID + 2]);
    }


    GeoPoolVertex FetchVertex(GeoPoolMeshChunk meshInfo, uint vertexIndex)
    {
        GeoPoolVertex v;
        GeometryPool::LoadVertex(meshInfo.vertexOffset + (int)vertexIndex, g_globalVertexBufferSize, 0, g_globalVertexBuffer, v);
        return v;
    }
}


static const uint kGeomAttribPosition = 1 << 0;
static const uint kGeomAttribNormal = 1 << 1;
static const uint kGeomAttribTexCoord0 = 1 << 4;
static const uint kGeomAttribTexCoord1 = 1 << 8;
static const uint kGeomAttribFaceNormal = 1 << 16;
static const uint kGeomAttribAll = 0xFFFFFFFF;

struct HitGeomAttributes
{
    float3 position;
    float3 normal;
    float3 faceNormal;
    float4 uv0;
    float4 uv1;
};

HitGeomAttributes FetchHitGeomAttributes(RayTracingAccelStruct accelStruct, Hit hit, uint attributesToFetch = kGeomAttribAll)
{
    HitGeomAttributes result = (HitGeomAttributes)0;

    int geometryIndex = accelStruct.instanceList[hit.instanceIndex].geometryIndex;
    GeoPoolMeshChunk meshInfo = g_MeshList[geometryIndex];
    uint3 triangleVertexIndices = Internal::FetchTriangleIndices(meshInfo, hit.primitiveIndex);

    GeoPoolVertex v0, v1, v2;
    v0 = Internal::FetchVertex(meshInfo, triangleVertexIndices.x);
    v1 = Internal::FetchVertex(meshInfo, triangleVertexIndices.y);
    v2 = Internal::FetchVertex(meshInfo, triangleVertexIndices.z);

    GeoPoolVertex v = Internal::InterpolateVertices(v0, v1, v2, hit.uvBarycentrics);

    if (attributesToFetch & kGeomAttribFaceNormal)
        result.faceNormal = cross(v1.pos - v0.pos, v2.pos - v0.pos);

    if (attributesToFetch & kGeomAttribPosition)
        result.position = v.pos;

    if (attributesToFetch & kGeomAttribNormal)
        result.normal = v.N;

    if (attributesToFetch & kGeomAttribTexCoord0)
        result.uv0 = v.uv0;

    if (attributesToFetch & kGeomAttribTexCoord0)
        result.uv1 = v.uv1;

    return result;
}

// For sampling geometry pool for a UV mesh that has position in UV and UV in positions - this is used for stochastic lightmap sampling
HitGeomAttributes UVFetchHitGeomAttributes(RayTracingAccelStruct accelStruct, Hit hit, uint attributesToFetch = kGeomAttribAll)
{
    HitGeomAttributes result = (HitGeomAttributes)0;

    int geometryIndex = accelStruct.instanceList[hit.instanceIndex].geometryIndex;
    GeoPoolMeshChunk meshInfo = g_MeshList[geometryIndex];
    uint3 triangleVertexIndices = Internal::FetchTriangleIndices(meshInfo, hit.primitiveIndex);

    GeoPoolVertex v0, v1, v2;
    v0 = Internal::FetchVertex(meshInfo, triangleVertexIndices.x);
    v1 = Internal::FetchVertex(meshInfo, triangleVertexIndices.y);
    v2 = Internal::FetchVertex(meshInfo, triangleVertexIndices.z);

    GeoPoolVertex v = Internal::InterpolateVertices(v0, v1, v2, hit.uvBarycentrics);

    if (attributesToFetch & kGeomAttribFaceNormal)
        result.faceNormal = cross(v1.uv0.xyz - v0.uv0.xyz, v2.uv0.xyz - v0.uv0.xyz);

    if (attributesToFetch & kGeomAttribPosition)
        result.position = v.uv0.xyz;

    if (attributesToFetch & kGeomAttribNormal)
        result.normal = v.N;

    if (attributesToFetch & kGeomAttribTexCoord0)
        result.uv0 = float4(v.pos.xyz, 0.0);

    if (attributesToFetch & kGeomAttribTexCoord1)
        result.uv1 = v.uv1;
        
    return result;
}

InstanceData GetInstance(RayTracingAccelStruct accelStruct, uint instanceIndex)
{
    return accelStruct.instanceList[instanceIndex];
}

} // namespace UnifiedRT

#endif // UNIFIEDRAYTRACING_FETCH_GEOMETRY_HLSL
