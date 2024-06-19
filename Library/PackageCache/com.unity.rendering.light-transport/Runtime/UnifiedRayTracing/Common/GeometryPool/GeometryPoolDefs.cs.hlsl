#ifndef GEOMETRYPOOLDEFS_CS_HLSL
#define GEOMETRYPOOLDEFS_CS_HLSL
//
// UnityEngine.Rendering.UnifiedRayTracing.GeoPoolVertexAttribs:  static fields
//
#define GEOPOOLVERTEXATTRIBS_POSITION (1)
#define GEOPOOLVERTEXATTRIBS_NORMAL (2)
#define GEOPOOLVERTEXATTRIBS_UV0 (4)
#define GEOPOOLVERTEXATTRIBS_UV1 (8)

//
// UnityEngine.Rendering.UnifiedRayTracing.GeometryPoolConstants:  static fields
//
#define GEO_POOL_POS_BYTE_SIZE (12)
#define GEO_POOL_UV0BYTE_SIZE (16)
#define GEO_POOL_UV1BYTE_SIZE (16)
#define GEO_POOL_NORMAL_BYTE_SIZE (4)
#define GEO_POOL_POS_BYTE_OFFSET (0)
#define GEO_POOL_UV0BYTE_OFFSET (12)
#define GEO_POOL_UV1BYTE_OFFSET (28)
#define GEO_POOL_NORMAL_BYTE_OFFSET (44)
#define GEO_POOL_INDEX_BYTE_SIZE (4)
#define GEO_POOL_VERTEX_BYTE_SIZE (48)

// Generated from UnityEngine.Rendering.UnifiedRayTracing.GeoPoolMeshChunk
struct GeoPoolMeshChunk
{
    int indexOffset;
    int indexCount;
    int vertexOffset;
    int vertexCount;
};

// Generated from UnityEngine.Rendering.UnifiedRayTracing.GeoPoolVertex
struct GeoPoolVertex
{
    float3 pos;
    float4 uv0;
    float4 uv1;
    float3 N;
};


#endif
