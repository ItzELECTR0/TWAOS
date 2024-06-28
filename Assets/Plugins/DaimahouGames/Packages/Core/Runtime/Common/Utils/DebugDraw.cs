// Remi Gillig - http://speps.fr - 2012 - Public domain

using System.Collections.Generic;
using DaimahouGames.Runtime.Core.Common;
using UnityEngine;

public static class DebugDraw
{
    private static MeshCreator m_Creator = new();
    
    private static Material m_Material;
    private static Mesh m_SolidSphere;
    private static Mesh m_SolidCube;
    private static Mesh m_SolidCylinder;
    
    private static readonly int COLOR = Shader.PropertyToID("_Color");
    private static readonly int MODE = Shader.PropertyToID("_Mode");
    private static readonly int SRC_BLEND = Shader.PropertyToID("_SrcBlend");
    private static readonly int DST_BLEND = Shader.PropertyToID("_DstBlend");
    private static readonly int Z_WRITE = Shader.PropertyToID("_ZWrite");

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        m_Creator = new MeshCreator();
        m_SolidSphere = m_Creator.CreateSphere(3);
        m_SolidCube = m_Creator.CreateCube(0);
        m_SolidCube = m_Creator.CreateCylinder(3);
        
        m_Material = new Material(Shader.Find("Standard"));
        m_Material.SetFloat(MODE, 3);
        m_Material.SetInt(SRC_BLEND, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m_Material.SetInt(DST_BLEND, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m_Material.SetInt(Z_WRITE, 1);
        m_Material.DisableKeyword("_ALPHATEST_ON");
        m_Material.EnableKeyword("_ALPHABLEND_ON");
        m_Material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m_Material.renderQueue = 3000;
    }

    public static async void DrawSphere(Vector3 position, float radius, Color color, float duration)
    {
        var start = Time.time;
        while (Time.time - start < duration)
        {
            DrawSphere(position, radius, color);
            await Awaiters.NextFrame;
        }
    }

    public static void DrawSphere(Vector3 position, float radius, Color color)
    {
        var mat = Matrix4x4.TRS(position, Quaternion.identity, radius * 0.5f * Vector3.one);
        var block = new MaterialPropertyBlock();

        block.SetColor(COLOR, color);
        Graphics.DrawMesh(m_SolidSphere, mat, m_Material, 0, null, 0, block);
    }

    public static void DrawCube(Vector3 position, Quaternion rotation, float size, Color color)
    {
        var mat = Matrix4x4.TRS(position, rotation, size * Vector3.one);
        var block = new MaterialPropertyBlock();
        
        block.SetColor(COLOR, color);
        Graphics.DrawMesh(m_SolidCube, mat, m_Material, 0, null, 0, block);
    }
    
    public static void DrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
    {
        var mat = Matrix4x4.TRS(position, rotation, scale);
        var block = new MaterialPropertyBlock();
        
        block.SetColor(COLOR, color);
        Graphics.DrawMesh(m_SolidCylinder, mat, m_Material, 0, null, 0, block);
    }

    private class MeshCreator
    {
        private List<Vector3> m_Positions;
        private List<Vector2> m_Uvs;
        private int m_Index;
        private Dictionary<long, int> m_MiddlePointIndexCache;

        // add vertex to mesh, fix position to be on unit sphere, return index
        private int AddVertex(Vector3 p, Vector2 uv)
        {
            m_Positions.Add(p);
            m_Uvs.Add(uv);
            return m_Index++;
        }

        // return index of point in the middle of p1 and p2
        private int GetMiddlePoint(int p1, int p2)
        {
            // first check if we have it already
            var firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            var greaterIndex = firstIsSmaller ? p2 : p1;
            var key = (smallerIndex << 32) + greaterIndex;

            if (m_MiddlePointIndexCache.TryGetValue(key, out var value))
            {
                return value;
            }

            // not in cache, calculate it
            var point1 = m_Positions[p1];
            var point2 = m_Positions[p2];
            var middle = new Vector3(
                (point1.x + point2.x) / 2.0f,
                (point1.y + point2.y) / 2.0f,
                (point1.z + point2.z) / 2.0f);

            var uv1 = m_Uvs[p1];
            var uv2 = m_Uvs[p2];
            var uvMiddle = new Vector2(
                (uv1.x + uv2.x) / 2.0f,
                (uv1.y + uv2.y) / 2.0f);

            // add vertex makes sure point is on unit sphere
            var i = AddVertex(middle, uvMiddle);

            // store it, return index
            m_MiddlePointIndexCache.Add(key, i);
            return i;
        }

        public Mesh CreateCube(int subdivisions)
        {
            m_Positions = new List<Vector3>();
            m_Uvs = new List<Vector2>();
            m_MiddlePointIndexCache = new Dictionary<long, int>();
            m_Index = 0;

            var indices = new List<int>();

            // front
            AddVertex(new Vector3(-1, -1, 1), new Vector2(1, 0));
            AddVertex(new Vector3(-1, 1, 1), new Vector2(1, 1));
            AddVertex(new Vector3(1, 1, 1), new Vector2(0, 1));
            AddVertex(new Vector3(1, -1, 1), new Vector2(0, 0));
            indices.Add(0); indices.Add(3); indices.Add(2);
            indices.Add(2); indices.Add(1); indices.Add(0);

            // right
            AddVertex(new Vector3(1, -1, 1), new Vector2(1, 0));
            AddVertex(new Vector3(1, 1, 1), new Vector2(1, 1));
            AddVertex(new Vector3(1, 1, -1), new Vector2(0, 1));
            AddVertex(new Vector3(1, -1, -1), new Vector2(0, 0));
            indices.Add(4); indices.Add(7); indices.Add(6);
            indices.Add(6); indices.Add(5); indices.Add(4);

            // back
            AddVertex(new Vector3(1, -1, -1), new Vector2(1, 0));
            AddVertex(new Vector3(1, 1, -1), new Vector2(1, 1));
            AddVertex(new Vector3(-1, 1, -1), new Vector2(0, 1));
            AddVertex(new Vector3(-1, -1, -1), new Vector2(0, 0));
            indices.Add(8); indices.Add(11); indices.Add(10);
            indices.Add(10); indices.Add(9); indices.Add(8);

            // left
            AddVertex(new Vector3(-1, -1, -1), new Vector2(1, 0));
            AddVertex(new Vector3(-1, 1, -1), new Vector2(1, 1));
            AddVertex(new Vector3(-1, 1, 1), new Vector2(0, 1));
            AddVertex(new Vector3(-1, -1, 1), new Vector2(0, 0));
            indices.Add(12); indices.Add(15); indices.Add(14);
            indices.Add(14); indices.Add(13); indices.Add(12);

            // top
            AddVertex(new Vector3(1, 1, 1), new Vector2(0, 0));
            AddVertex(new Vector3(1, 1, -1), new Vector2(0, 1));
            AddVertex(new Vector3(-1, 1, -1), new Vector2(1, 1));
            AddVertex(new Vector3(-1, 1, 1), new Vector2(1, 0));
            indices.Add(16); indices.Add(17); indices.Add(18);
            indices.Add(18); indices.Add(19); indices.Add(16);

            // bottom
            AddVertex(new Vector3(1, -1, 1), new Vector2(1, 0));
            AddVertex(new Vector3(1, -1, -1), new Vector2(1, 1));
            AddVertex(new Vector3(-1, -1, -1), new Vector2(0, 1));
            AddVertex(new Vector3(-1, -1, 1), new Vector2(0, 0));
            indices.Add(21); indices.Add(20); indices.Add(23);
            indices.Add(23); indices.Add(22); indices.Add(21);

            for (var i = 0; i < subdivisions; i++)
            {
                var indices2 = new List<int>();
                for (var idx = 0; idx < indices.Count; idx += 3)
                {
                    // replace triangle by 4 triangles
                    var a = GetMiddlePoint(indices[idx + 0], indices[idx + 1]);
                    var b = GetMiddlePoint(indices[idx + 1], indices[idx + 2]);
                    var c = GetMiddlePoint(indices[idx + 2], indices[idx + 0]);

                    indices2.Add(indices[idx + 0]); indices2.Add(a); indices2.Add(c);
                    indices2.Add(indices[idx + 1]); indices2.Add(b); indices2.Add(a);
                    indices2.Add(indices[idx + 2]); indices2.Add(c); indices2.Add(b);
                    indices2.Add(a); indices2.Add(b); indices2.Add(c);
                }
                indices = indices2;
            }

            // done, create the mesh
            var mesh = new Mesh
            {
                vertices = m_Positions.ToArray(),
                triangles = indices.ToArray(),
                uv = m_Uvs.ToArray()
            };

            mesh.RecalculateNormals();

            var colors = new Color[mesh.vertexCount];
            for (var i = 0; i < colors.Length; i++)
                colors[i] = new Color(1.0f, 1.0f, 1.0f);
            mesh.colors = colors;

            RecalculateTangents(mesh);

            return mesh;
        }

        public Mesh CreateSphere(int subdivisions)
        {
            var sphere = CreateCube(subdivisions);
            var vertices = new List<Vector3>(sphere.vertices);

            for (var i = 0; i < vertices.Count; i++)
                vertices[i] = vertices[i].normalized;

            sphere.vertices = vertices.ToArray();
            sphere.RecalculateNormals();
            RecalculateTangents(sphere);

            return sphere;
        }

        public Mesh CreateCylinder(int subdivisions)
        {
            var cylinder = CreateCube(subdivisions);
            var vertices = new List<Vector3>(cylinder.vertices);

            for (int i = 0; i < vertices.Count; i++) 
            {
                Vector3 pos = new Vector3(vertices[i].x, 0.0f, vertices[i].z).normalized;
                pos.y = vertices[i].y;
                vertices[i] = pos;
            }

            cylinder.vertices = vertices.ToArray();
            cylinder.RecalculateNormals();
            RecalculateTangents(cylinder);

            return cylinder;
        }

        // Lengyel, Eric. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”.
        // Terathon Software 3D Graphics Library, 2001. http://www.terathon.com/code/tangent.html
        public static void RecalculateTangents(Mesh mesh)
        {
            var tan1 = new Vector3[mesh.vertexCount];
            var tan2 = new Vector3[mesh.vertexCount];
    
            for (var a = 0; a < mesh.triangles.Length; a += 3)
            {
                var i1 = mesh.triangles[a + 0];
                var i2 = mesh.triangles[a + 1];
                var i3 = mesh.triangles[a + 2];
        
                var v1 = mesh.vertices[i1];
                var v2 = mesh.vertices[i2];
                var v3 = mesh.vertices[i3];
        
                var w1 = mesh.uv[i1];
                var w2 = mesh.uv[i2];
                var w3 = mesh.uv[i3];
        
                var x1 = v2.x - v1.x;
                var x2 = v3.x - v1.x;
                var y1 = v2.y - v1.y;
                var y2 = v3.y - v1.y;
                var z1 = v2.z - v1.z;
                var z2 = v3.z - v1.z;
        
                var s1 = w2.x - w1.x;
                var s2 = w3.x - w1.x;
                var t1 = w2.y - w1.y;
                var t2 = w3.y - w1.y;
        
                var r = 1.0F / (s1 * t2 - s2 * t1);
                var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r,
                        (t2 * z1 - t1 * z2) * r);
                var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r,
                        (s1 * z2 - s2 * z1) * r);
        
                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;
        
                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            var tangents = new Vector4[mesh.vertexCount];
            for (long a = 0; a < mesh.vertexCount; a++)
            {
                var n = mesh.normals[a];
                var t = tan1[a];
        
                // Gram-Schmidt orthogonalize
                tangents[a] = t - n * Vector3.Dot(n, t);
                tangents[a].Normalize();
        
                // Calculate handedness
                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

            mesh.tangents = tangents;
        }
    }
}