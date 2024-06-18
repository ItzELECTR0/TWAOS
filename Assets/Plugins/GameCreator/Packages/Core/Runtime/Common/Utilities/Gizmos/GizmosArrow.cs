using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static partial class GizmosExtension
	{
        private static Mesh ARROW_MESH;

        private static readonly Vector2[] ARROW_MESH_OUTLINE =
        {
            new Vector2(0,1), new Vector2(1,2), new Vector2(2,3),
            new Vector2(3,4), new Vector2(4,5), new Vector2(5,6),
            new Vector2(6,0)
        };

        private static readonly Color ARROW_COLOR_FILL = new Color(0, 0, 0, 0.2f);

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Arrow(Vector3 position, Vector3 direction, float size = 1f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            Arrow(position, rotation, size);
        }

        public static void Arrow(Vector3 position, Quaternion rotation, float size = 1f)
        {
            Mesh mesh = GetArrowMesh();

            Color color = Gizmos.color;
            Gizmos.color = ARROW_COLOR_FILL;

            Gizmos.DrawMesh(mesh, position, rotation, Vector3.one * size);
            Gizmos.color = color;

            for (int i = 0; i < ARROW_MESH_OUTLINE.Length; ++i)
            {
                Gizmos.DrawLine(
                    position + rotation * mesh.vertices[(int) ARROW_MESH_OUTLINE[i].x] * size,
                    position + rotation * mesh.vertices[(int) ARROW_MESH_OUTLINE[i].y] * size
                );
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Mesh GetArrowMesh()
        {
            if (ARROW_MESH != null) return ARROW_MESH;

            Vector3[] vertices = 
            {
                new Vector3( 0.0f, 0f,  1f),
                new Vector3(-1.0f, 0f,  0f),
                new Vector3(-0.5f, 0f,  0f),
                new Vector3(-0.5f, 0f, -1f),
                new Vector3( 0.5f, 0f, -1f),
                new Vector3( 0.5f, 0f,  0f),
                new Vector3( 1.0f, 0f,  0f)
            };

            int[] triangles = 
            {
                0, 6, 5,
                0, 5, 4,
                0, 4, 3,
                0, 3, 2,
                0, 2, 1
            };

            Vector2[] uvs =
            {
                new Vector2(0.5f, 1.0f),
                new Vector2(1.0f, 0.5f),
                new Vector2(.25f, 0.5f),
                new Vector2(.25f, 0.0f),
                new Vector2(.75f, 0.0f),
                new Vector2(.75f, 0.5f),
                new Vector2(1.0f, 0.5f)
            };

            Vector3[] normals =
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals,
                uv = uvs
            };

            ARROW_MESH = mesh;
            return mesh;
        }
    }
}