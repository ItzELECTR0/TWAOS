using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static partial class GizmosExtension
	{
		private static Mesh CIRCLE_SOLID;
		private const int CIRCLE_SEGMENTS = 90;
		
		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public static void Circle(Vector3 position, float diameter, bool solid = false)
		{
			Circle(position, diameter, Vector3.up, solid);
		}

        public static void Circle(Vector3 position, float diameter, Vector3 normal, bool solid = false)
        {
	        switch (solid)
	        {
		        case true: CircleSolid(position, diameter, normal); break;
		        case false: CircleWire(position, diameter, normal); break;
	        }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void CircleSolid(Vector3 position, float radius, Vector3 normal)
        {
	        Mesh mesh = GetCircleSolidMesh();

	        Color color = Gizmos.color;
	        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
	        Gizmos.DrawMesh(mesh, position, rotation, Vector3.one * radius);
	        Gizmos.color = color;
        }

        private static void CircleWire(Vector3 position, float radius, Vector3 normal)
        {
	        Vector3 top = normal.normalized * radius;
	        Vector3 fwd = Vector3.Slerp(top, -top, 0.5f);
	        Vector3 rht = Vector3.Cross(top, fwd).normalized * radius;

	        Matrix4x4 matrix = new Matrix4x4
	        {
		        [0]  = rht.x,
		        [1]  = rht.y,
		        [2]  = rht.z,
		        [4]  = top.x,
		        [5]  = top.y,
		        [6]  = top.z,
		        [8]  = fwd.x,
		        [9]  = fwd.y,
		        [10] = fwd.z
	        };

	        Vector3 prevPoint = position + matrix.MultiplyPoint3x4(Vector3.right);
	        Vector3 nextPoint = Vector3.zero;

	        for (int i = 0; i < CIRCLE_SEGMENTS + 1; i++)
	        {
		        nextPoint.x = Mathf.Cos(i * 4 * Mathf.Deg2Rad);
		        nextPoint.z = Mathf.Sin(i * 4 * Mathf.Deg2Rad);
		        nextPoint.y = 0;

		        nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);

		        Gizmos.DrawLine(prevPoint, nextPoint);
		        prevPoint = nextPoint;
	        }
        }
        
        private static Mesh GetCircleSolidMesh()
        {
            if (CIRCLE_SOLID != null) return CIRCLE_SOLID;
			
            List<Vector3> vertices = new List<Vector3>(CIRCLE_SEGMENTS + 2);
            int[] indices = new int[CIRCLE_SEGMENTS * 3];
            
            const float segmentWidth = Mathf.PI * 2f / CIRCLE_SEGMENTS;
            float angle = 0f;
            
            vertices.Add(Vector3.zero);
            
            for (int i = 1; i < CIRCLE_SEGMENTS + 2; ++i)
            {
	            vertices.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
                
                angle -= segmentWidth;
                if (i <= 1) continue;
                
                int j = (i - 2) * 3;
                indices[j + 0] = 0;
                indices[j + 1] = i - 1;
                indices[j + 2] = i;
            }

            Mesh mesh = new Mesh();
            
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            CIRCLE_SOLID = mesh;
            return mesh;
        }
    }
}