using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
    {
        private static readonly Vector3[] DIRECTIONS = {
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.forward
        };

        private static readonly Mesh[] OCTAHEDRON_MESHES = new Mesh[MAX_SUBDIVISIONS + 1];

        private const int MIN_SUBDIVISIONS = 0;
        private const int MAX_SUBDIVISIONS = 6;

        private const int DEFAULT = 5;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Octahedron(Vector3 origin, Quaternion rotation,
            float radius, int subdivisions = DEFAULT)
        {
            Gizmos.DrawMesh(
                RequestOctahedron(subdivisions),
                origin, rotation, Vector3.one * radius
            );
        }

        public static void OctahedronWire(Vector3 origin, Quaternion rotation,
            float radius, int subdivisions = DEFAULT)
        {
            Gizmos.DrawWireMesh(
                RequestOctahedron(subdivisions),
                origin, rotation, Vector3.one * radius
            );
        }

        // MESH METHODS: --------------------------------------------------------------------------

        private static Mesh RequestOctahedron(int subdivisions)
        {
            subdivisions = Mathf.Clamp(subdivisions, MIN_SUBDIVISIONS, MAX_SUBDIVISIONS);
            if (OCTAHEDRON_MESHES[subdivisions] == null)
            {
                OCTAHEDRON_MESHES[subdivisions] = CreateOctahedron(subdivisions);
            }

            return OCTAHEDRON_MESHES[subdivisions];
        }

        private static Mesh CreateOctahedron(int subdivisions)
        {
            subdivisions = Mathf.Clamp(subdivisions, MIN_SUBDIVISIONS, MAX_SUBDIVISIONS);

            int resolution = 1 << subdivisions;
            int size = (resolution + 1) * (resolution + 1) * 4 - (resolution * 2 - 1) * 3;
            Vector3[] vertices = new Vector3[size];
            int[] triangles = new int[(1 << (subdivisions * 2 + 3)) * 3];

            CreateOctahedronVertices(vertices, triangles, resolution);

            Vector3[] normals = new Vector3[vertices.Length];
            OctahedronNormalize(vertices, normals);

            Mesh mesh = new Mesh
            {
                name = "Octahedron",
                vertices = vertices,
                normals = normals,
                triangles = triangles
            };

            return mesh;
        }

        private static void CreateOctahedronVertices(Vector3[] vertices, int[] triangles, int resolution)
        {
            int verticesIndex = 0;
            int verticesBottom = 0;

            int t = 0;

            for (int i = 0; i < 4; i++)
            {
                vertices[verticesIndex++] = Vector3.down;
            }

            for (int i = 1; i <= resolution; i++)
            {
                float progress = (float)i / resolution;
                Vector3 from;
                Vector3 to;

                vertices[verticesIndex++] = to = Vector3.Lerp(
                    Vector3.down, 
                    Vector3.forward, 
                    progress
                );

                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.down, DIRECTIONS[d], progress);
                    t = CreateOctahedronLowerStrip(i, verticesIndex, verticesBottom, t, triangles);
                    verticesIndex = CreateOctahedronVertexLine(from, to, i, verticesIndex, vertices);
                    verticesBottom += i > 1 ? (i - 1) : 1;
                }

                verticesBottom = verticesIndex - 1 - i * 4;
            }

            for (int i = resolution - 1; i >= 1; i--)
            {
                float progress = (float)i / resolution;
                Vector3 from;
                Vector3 to;

                vertices[verticesIndex++] = to = Vector3.Lerp(
                    Vector3.up, 
                    Vector3.forward,
                    progress
                );
                
                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.up, DIRECTIONS[d], progress);

                    t = CreateOctahedronUpperStrip(i, verticesIndex, verticesBottom, t, triangles);

                    verticesIndex = CreateOctahedronVertexLine(from, to, i, verticesIndex, vertices);
                    verticesBottom += i + 1;
                }

                verticesBottom = verticesIndex - 1 - i * 4;
            }

            for (int i = 0; i < 4; i++)
            {
                triangles[t++] = verticesBottom;
                triangles[t++] = verticesIndex;
                triangles[t++] = ++verticesBottom;
                vertices[verticesIndex++] = Vector3.up;
            }
        }

        private static int CreateOctahedronVertexLine(Vector3 from, Vector3 to, int steps, int v,
            Vector3[] vertices)
        {
            for (int i = 1; i <= steps; i++)
            {
                vertices[v++] = Vector3.Lerp(from, to, (float)i / steps);
            }

            return v;
        }

        private static int CreateOctahedronLowerStrip(int steps, int vTop, int vBottom, int t, 
            int[] triangles)
        {
            for (int i = 1; i < steps; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;

                triangles[t++] = vBottom++;
                triangles[t++] = vTop++;
                triangles[t++] = vBottom;
            }

            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            return t;
        }

        private static int CreateOctahedronUpperStrip(int steps, int vTop, int vBottom, int t,
            int[] triangles)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = ++vBottom;

            for (int i = 1; i <= steps; i++)
            {
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;
                triangles[t++] = vBottom;

                triangles[t++] = vBottom;
                triangles[t++] = vTop++;
                triangles[t++] = ++vBottom;
            }
            return t;
        }

        private static void OctahedronNormalize(Vector3[] vertices, Vector3[] normals)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = vertices[i] = vertices[i].normalized;
            }
        }
    }
}