using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.RadeonRays
{

    internal class AABB
    {
        public float3 Min;
        public float3 Max;

        public AABB()
        {
            Min = new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Max = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        }

        public AABB(float3 min, float3 max)
        {
            Min = min;
            Max = max;
        }

        public void Encapsulate(AABB aabb)
        {
            Min = math.min(Min, aabb.Min);
            Max = math.max(Max, aabb.Max);
        }

        public void Encapsulate(float3 point)
        {
            Min = math.min(Min, point);
            Max = math.max(Max, point);
        }

        public bool Contains(AABB rhs)
        {
            return rhs.Min.x >= Min.x && rhs.Min.y >= Min.y && rhs.Min.z >= Min.z &&
                   rhs.Max.x <= Max.x && rhs.Max.y <= Max.y && rhs.Max.z <= Max.z;
        }

        public bool IsValid()
        {
            return Min.x <= Max.x && Min.y <= Max.y && Min.z <= Max.z;
        }
    }


    internal class BvhCheck
    {
        const uint kInvalidID = ~0u;

        public class VertexBuffers
        {
            public GraphicsBuffer vertices;
            public GraphicsBuffer indices;
            public uint vertexBufferOffset = 0;
            public uint vertexCount;
            public uint vertexStride = 3;
            public uint indexBufferOffset = 0;
            public uint indexCount;
        };

        public static double SurfaceArea(AABB aabb)
        {
            float3 edges = aabb.Max - aabb.Min;
            return 2.0f * (edges.x * edges.y + edges.x * edges.z + edges.z * edges.y);
        }

        public static double NodeSahCost(BvhNode node, AABB nodeAabb, AABB parentAabb)
        {
            double cost = node.child0 == kInvalidID ? node.child1 : 1.2f;
            return cost * SurfaceArea(nodeAabb) / SurfaceArea(parentAabb);
        }

        public static double CheckConsistency(VertexBuffers bvhVertexBuffers, GraphicsBuffer bvhBuffer, uint bvhBufferOffset, uint primitiveCount)
        {
            var header = new BvhHeader[1];
            bvhBuffer.GetData(header, 0, (int)bvhBufferOffset, 1);

            return CheckConsistency(bvhVertexBuffers, bvhBuffer, bvhBufferOffset + 1, header[0].leafNodeCount, header[0].root, primitiveCount);
        }


        public static double CheckConsistency(
            VertexBuffers bvhVertexBuffers, GraphicsBuffer bvhBuffer,
            uint bvhBufferOffset, uint leafCount, uint rootAddr, uint primitiveCount)
        {
            var nodeCount = HlbvhBuilder.GetBvhNodeCount(leafCount);

            var bvhNodes = new BvhNode[nodeCount];
            bvhBuffer.GetData(bvhNodes, 0, (int)bvhBufferOffset, (int)nodeCount);

            bool isTopLevel = bvhVertexBuffers == null;
            VertexBuffersCPU vertexBuffers = null;
            if (!isTopLevel)
                vertexBuffers = DownloadVertexData(bvhVertexBuffers);

            uint countedPrimitives = 0;

            var rootAabb = GetAabb(vertexBuffers, bvhNodes[rootAddr], isTopLevel);
            double sahCost = 0.0f;

            var q = new Queue<(uint Addr, uint Parent)>();
            q.Enqueue((Addr: rootAddr, Parent: kInvalidID));
            while (q.Count != 0)
            {
                var current = q.Dequeue();
                uint addr = current.Addr;
                uint parent = current.Parent;
                var node = bvhNodes[addr];
                AABB aabb = GetAabb(vertexBuffers, node, isTopLevel);
                sahCost += NodeSahCost(node, aabb, rootAabb);

                Assert.AreEqual(parent, node.parent);
                Assert.IsTrue(aabb.IsValid());

                if (node.child0 != kInvalidID)
                {
                    var leftAabb = GetAabb(vertexBuffers, bvhNodes[node.child0], isTopLevel);
                    var rightAabb = GetAabb(vertexBuffers, bvhNodes[node.child1], isTopLevel);

                    bool leftOk = (aabb.Contains(leftAabb));
                    bool rightOk = (aabb.Contains(rightAabb));

                    Assert.IsTrue(leftOk);
                    Assert.IsTrue(rightOk);

                    q.Enqueue((Addr: node.child0, Parent: addr));
                    q.Enqueue((Addr: node.child1, Parent: addr));
                }
                else // leaf
                {
                    countedPrimitives += isTopLevel ? 1 : node.aabb0_min[0];
                }
            }

            Assert.AreEqual(countedPrimitives, primitiveCount);

            return sahCost;
        }

        private class VertexBuffersCPU
        {
            public float[] vertices;
            public uint[] indices;
            public uint vertexStride;
        };


        static uint3 GetFaceIndices(uint[] indices, uint triangleIdx)
        {
            return new uint3(
                indices[3 * triangleIdx],
                indices[3 * triangleIdx + 1],
                indices[3 * triangleIdx + 2]);
        }

        static float3 GetVertex(float[] vertices, uint stride, uint idx)
        {
            uint indexInFloats = idx * stride;
            return new float3(
                vertices[indexInFloats],
                vertices[indexInFloats + 1],
                vertices[indexInFloats + 2]);
        }

        struct Triangle
        {
            public float3 v0;
            public float3 v1;
            public float3 v2;
        };

        static Triangle GetTriangle(float[] vertices, uint stride, uint3 idx)
        {
            Triangle tri;
            tri.v0 = GetVertex(vertices, stride, idx.x);
            tri.v1 = GetVertex(vertices, stride, idx.y);
            tri.v2 = GetVertex(vertices, stride, idx.z);
            return tri;
        }

        static VertexBuffersCPU DownloadVertexData(VertexBuffers vertexBuffers)
        {
            var result = new VertexBuffersCPU();
            result.vertices = new float[vertexBuffers.vertexCount * vertexBuffers.vertexStride];
            result.indices = new uint[vertexBuffers.indexCount];
            result.vertexStride = vertexBuffers.vertexStride;

            vertexBuffers.indices.GetData(result.indices, 0, (int)vertexBuffers.indexBufferOffset, (int)vertexBuffers.indexCount);

            vertexBuffers.vertices.GetData(result.vertices, 0, (int)vertexBuffers.vertexBufferOffset, (int)(vertexBuffers.vertexCount * vertexBuffers.vertexStride));

            return result;
        }

        static AABB GetAabb(VertexBuffersCPU bvhVertexBuffers, BvhNode node, bool isTopLevel)
        {
            var aabb = new AABB();

            if (node.child0 != kInvalidID || isTopLevel)
            {
                AABB left = new AABB(math.asfloat(node.aabb0_min), math.asfloat(node.aabb0_max));
                aabb.Encapsulate(left);

                AABB right = new AABB(math.asfloat(node.aabb1_min), math.asfloat(node.aabb1_max));
                aabb.Encapsulate(right);
            }
            else
            {
                int fisrtIndex = (int)node.child1;
                int triangleCount = (int)node.aabb0_min[0];
                for (int i = 0; i < triangleCount; ++i)
                {
                    uint index = (uint)(i + fisrtIndex);

                    var triangleIndices = GetFaceIndices(bvhVertexBuffers.indices, index);
                    var triangle = GetTriangle(bvhVertexBuffers.vertices, bvhVertexBuffers.vertexStride, triangleIndices);

                    aabb.Encapsulate(triangle.v0);
                    aabb.Encapsulate(triangle.v1);
                    aabb.Encapsulate(triangle.v2);
                }
            }

            return aabb;
        }
    }
}

