#ifndef GEOMETRY_POOL_H
#define GEOMETRY_POOL_H

namespace GeometryPool
{

float2 msign(float2 v)
{
    return float2(
        (v.x >= 0.0) ? 1.0 : -1.0,
        (v.y >= 0.0) ? 1.0 : -1.0);
}

uint NormalToOctahedral32(float3 normal)
{
    normal.xy /= (abs(normal.x) + abs(normal.y) + abs(normal.z));
    normal.xy = (normal.z >= 0.0) ? normal.xy : (1.0 - abs(normal.yx)) * msign(normal.xy);

    uint2 d = uint2(round(32767.5 + normal.xy * 32767.5));
    return d.x | (d.y << 16u);
}

float3 Octahedral32ToNormal(uint data)
{
    uint2 iv = uint2(data, data >> 16u) & 65535u;
    float2 v = float2(iv) / 32767.5 - 1.0;

    float3 normal = float3(v, 1.0 - abs(v.x) - abs(v.y));
    float t = max(-normal.z, 0.0);
    normal.x += (normal.x > 0.0) ? -t : t;
    normal.y += (normal.y > 0.0) ? -t : t;

    return normalize(normal);
}

void StoreVertex(
    uint vertexIndex,
    in GeoPoolVertex vertex,
    int outputBufferSize,
    RWStructuredBuffer<uint> output)
{
    uint posIndex = vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE / 4;
    output[posIndex] = asuint(vertex.pos.x);
    output[posIndex+1] = asuint(vertex.pos.y);
    output[posIndex+2] = asuint(vertex.pos.z);

    uint uv0Index = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_UV0BYTE_OFFSET) / 4;
    output[uv0Index] = asuint(vertex.uv0.x);
    output[uv0Index + 1] = asuint(vertex.uv0.y);
    output[uv0Index + 2] = asuint(vertex.uv0.z);
    output[uv0Index + 3] = asuint(vertex.uv0.w);

    uint uv1Index = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_UV1BYTE_OFFSET) / 4;
    output[uv1Index] = asuint(vertex.uv1.x);
    output[uv1Index + 1] = asuint(vertex.uv1.y);
    output[uv1Index + 2] = asuint(vertex.uv1.z);
    output[uv1Index + 3] = asuint(vertex.uv1.w);

    uint normalIndex = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_NORMAL_BYTE_OFFSET) / 4;
    output[normalIndex] = NormalToOctahedral32(vertex.N);
}

void LoadVertex(
    uint vertexIndex,
    int vertexBufferSize,
    int vertexFlags,
    StructuredBuffer<uint> vertexBuffer,
    out GeoPoolVertex outputVertex)
{
    uint posIndex = vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE / 4;
    float3 pos = asfloat(uint3(vertexBuffer[posIndex], vertexBuffer[posIndex + 1], vertexBuffer[posIndex + 2]));

    uint uv0Index = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_UV0BYTE_OFFSET) / 4;
    float4 uv0 = asfloat(uint4(vertexBuffer[uv0Index], vertexBuffer[uv0Index + 1], vertexBuffer[uv0Index + 2], vertexBuffer[uv0Index + 3]));

    uint uv1Index = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_UV1BYTE_OFFSET) / 4;
    float4 uv1 = asfloat(uint4(vertexBuffer[uv1Index], vertexBuffer[uv1Index + 1], vertexBuffer[uv1Index + 2], vertexBuffer[uv1Index + 3]));

    uint normalIndex = (vertexIndex * GEO_POOL_VERTEX_BYTE_SIZE + GEO_POOL_NORMAL_BYTE_OFFSET) / 4;
    uint normal = uint(vertexBuffer[normalIndex]);

    outputVertex.pos = pos;
    outputVertex.uv0 = uv0;
    outputVertex.uv1 = uv1;
    outputVertex.N = Octahedral32ToNormal(normal);
}

}

#endif
