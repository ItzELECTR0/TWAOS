/**********************************************************************
Copyright (c) 2019 Advanced Micro Devices, Inc. All rights reserved.
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
********************************************************************/
#ifndef BVH2IL_HLSL
#define BVH2IL_HLSL

#ifndef HOST
#include "aabb.hlsl"
#endif

#define INVALID_IDX 0xffffffff

#define BVH_NODE_SIZE 64
#define BVH_NODE_STRIDE_SHIFT 6
#define BVH_NODE_COUNT(N) (2 * (N)-1)
#define BVH_NODE_BYTE_OFFSET(i) ((i) << BVH_NODE_STRIDE_SHIFT)

#define LEAF_NODE_INDEX(i, n) ((n - 1) + i)
#define INTERNAL_NODE_INDEX(i, n) (i)
#define IS_INTERNAL_NODE(i, n) ((i < n - 1) ? true : false)
#define IS_LEAF_NODE(i, n) (!IS_INTERNAL_NODE(i, n))

//< BVH2 node.
struct BvhNode
{
    uint child0;
    uint child1;
    uint parent;
    uint update;

    uint data[12];

    void SetLeftAabb(float3 min, float3 max)
    {
        data[0] = asuint(min.x); data[1] = asuint(min.y); data[2] = asuint(min.z);
        data[3] = asuint(max.x); data[4] = asuint(max.y); data[5] = asuint(max.z);
    }

    void SetLeftAabb(Aabb aabb)
    {
        SetLeftAabb(aabb.pmin, aabb.pmax);
    }

    void SetRightAabb(float3 min, float3 max)
    {
        data[6] = asuint(min.x); data[7] = asuint(min.y); data[8] = asuint(min.z);
        data[9] = asuint(max.x); data[10] = asuint(max.y); data[11] = asuint(max.z);
    }

    void SetRightAabb(Aabb aabb)
    {
        SetRightAabb(aabb.pmin, aabb.pmax);
    }

    float3 LeftAabbMin()
    {
        return float3(asfloat(data[0]), asfloat(data[1]), asfloat(data[2]));
    }

    float3 LeftAabbMax()
    {
        return float3(asfloat(data[3]), asfloat(data[4]), asfloat(data[5]));
    }

    float3 RightAabbMin()
    {
        return float3(asfloat(data[6]), asfloat(data[7]), asfloat(data[8]));
    }

    float3 RightAabbMax()
    {
        return float3(asfloat(data[9]), asfloat(data[10]), asfloat(data[11]));
    }
};

struct InstanceInfo
{
    int blas_offset;
    int instance_mask;
    int vertex_offset;
    int index_offset;
    int triangle_culling_enabled;
    int invert_triangle_culling;
    uint user_instance_id;
    int padding2;
    Transform world_to_local_transform;
    Transform local_to_world_transform;

};


#if TOP_LEVEL
//< Calculate BVH2 node bounding box.
Aabb GetNodeAabb(in BvhNode node, bool internal)
{
    Aabb aabb = CreateEmptyAabb();

    if (internal)
    {
        // 3 vertices or 3 points for both internal and leafs.
        GrowAabb(node.LeftAabbMin(), aabb);
        GrowAabb(node.LeftAabbMax(), aabb);
        GrowAabb(node.RightAabbMin(), aabb);
        GrowAabb(node.RightAabbMax(), aabb);
    }
    else
    {
        aabb.pmin = node.LeftAabbMin();
        aabb.pmax = node.LeftAabbMax();
    }

    return aabb;
}
#else

//< Calculate BVH2 node bounding box.
Aabb GetNodeAabb(in BvhNode node, bool internal)
{
    Aabb aabb = CreateEmptyAabb();

    if (internal)
    {
        GrowAabb(node.LeftAabbMin(), aabb);
        GrowAabb(node.LeftAabbMax(), aabb);
        GrowAabb(node.RightAabbMin(), aabb);
        GrowAabb(node.RightAabbMax(), aabb);
    }
    else
    {
        int firstTriangle = node.child1;
        int triangleCount = node.data[0];
        for (int i = 0; i < triangleCount; ++i)
        {
            uint3 indices = GetFaceIndices(firstTriangle+i);
            TriangleData tri = FetchTriangle(indices);

            GrowAabb(tri.v0, aabb);
            GrowAabb(tri.v1, aabb);
            GrowAabb(tri.v2, aabb);
        }
    }

    return aabb;
}
#endif

Aabb GetNodeAabb(in BvhNode node)
{
    return GetNodeAabb(node, node.child0 != INVALID_IDX);
}

Aabb GetNodeAabbSync(globallycoherent RWStructuredBuffer<BvhNode> g_bvh, int index, bool internal)
{
    Aabb aabb = CreateEmptyAabb();
#if !TOP_LEVEL
    if (internal)
#endif
    {
        uint3 aabb0_min;
        uint3 aabb0_max;
        uint3 aabb1_min;
        uint3 aabb1_max;

        InterlockedAdd(g_bvh[index].data[0], 0, aabb0_min.x);
        InterlockedAdd(g_bvh[index].data[1], 0, aabb0_min.y);
        InterlockedAdd(g_bvh[index].data[2], 0, aabb0_min.z);

        InterlockedAdd(g_bvh[index].data[3], 0, aabb0_max.x);
        InterlockedAdd(g_bvh[index].data[4], 0, aabb0_max.y);
        InterlockedAdd(g_bvh[index].data[5], 0, aabb0_max.z);

        InterlockedAdd(g_bvh[index].data[6], 0, aabb1_min.x);
        InterlockedAdd(g_bvh[index].data[7], 0, aabb1_min.y);
        InterlockedAdd(g_bvh[index].data[8], 0, aabb1_min.z);

        InterlockedAdd(g_bvh[index].data[9], 0, aabb1_max.x);
        InterlockedAdd(g_bvh[index].data[10], 0, aabb1_max.y);
        InterlockedAdd(g_bvh[index].data[11], 0, aabb1_max.z);

        GrowAabb(asfloat(aabb0_min), aabb);
        GrowAabb(asfloat(aabb0_max), aabb);
        GrowAabb(asfloat(aabb1_min), aabb);
        GrowAabb(asfloat(aabb1_max), aabb);
    }
#if !TOP_LEVEL
    else
    {

        BvhNode node = g_bvh[index];
        int firstTriangle = node.child1;
        int triangleCount = node.data[0];
        for (int i = 0; i < triangleCount; ++i)
        {
            uint3 indices = GetFaceIndices(firstTriangle+i);
            TriangleData tri = FetchTriangle(indices);

            GrowAabb(tri.v0, aabb);
            GrowAabb(tri.v1, aabb);
            GrowAabb(tri.v2, aabb);
        }

    }
#endif
    return aabb;
}

void SetNodeAabbsSync(globallycoherent RWStructuredBuffer<BvhNode> bvh, int index, Aabb aabb0, Aabb aabb1)
{
    uint old_value2 = 0;
    InterlockedExchange(bvh[index].data[0], asuint(aabb0.pmin.x), old_value2);
    InterlockedExchange(bvh[index].data[1], asuint(aabb0.pmin.y), old_value2);
    InterlockedExchange(bvh[index].data[2], asuint(aabb0.pmin.z), old_value2);
    InterlockedExchange(bvh[index].data[3], asuint(aabb0.pmax.x), old_value2);
    InterlockedExchange(bvh[index].data[4], asuint(aabb0.pmax.y), old_value2);
    InterlockedExchange(bvh[index].data[5], asuint(aabb0.pmax.z), old_value2);

    InterlockedExchange(bvh[index].data[6], asuint(aabb1.pmin.x), old_value2);
    InterlockedExchange(bvh[index].data[7], asuint(aabb1.pmin.y), old_value2);
    InterlockedExchange(bvh[index].data[8], asuint(aabb1.pmin.z), old_value2);
    InterlockedExchange(bvh[index].data[9], asuint(aabb1.pmax.x), old_value2);
    InterlockedExchange(bvh[index].data[10], asuint(aabb1.pmax.y), old_value2);
    InterlockedExchange(bvh[index].data[11], asuint(aabb1.pmax.z), old_value2);
}

#endif  // BVH2IL_HLSL
