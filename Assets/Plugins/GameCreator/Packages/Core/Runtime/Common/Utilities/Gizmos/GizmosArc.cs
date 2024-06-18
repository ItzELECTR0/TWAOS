using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
	{
		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public static void Arc(Vector3 position, Quaternion rotation, float angle, float minRadius, float maxRadius)
		{
			Mesh mesh = GetArcMesh(angle, minRadius, maxRadius);

			Color color = Gizmos.color;
			Gizmos.DrawMesh(mesh, position, rotation, Vector3.one);
			Gizmos.color = color;
		}
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Mesh GetArcMesh(float angle, float minRadius, float maxRadius)
        {
	        int segments = Mathf.FloorToInt(angle / 10f);
	        Mesh mesh = new Mesh
	        {
		        vertices = new Vector3[4 * segments],
		        triangles = new int[3 * 2 * segments]
	        };

	        Vector3[] normals = new Vector3[4 * segments];
	        Vector2[] uv = new Vector2[4 * segments];
	        
	        Vector3[] vertices = new Vector3[4 * segments];
	        int[] triangles = new int[3 * 2 * segments];

	        for (int i = 0; i < uv.Length; i++)
	        {
		        uv[i] = new Vector2(0, 0);
	        }

	        for (int i = 0; i < normals.Length; i++)
	        {
		        normals[i] = new Vector3(0, 1, 0);
	        }
		    
		    mesh.uv = uv;
		    mesh.normals = normals;
		    
		    float angle1 = -angle / 2;
		    float angle2 = +angle / 2;
		    float delta = (angle2 - angle1) / segments;

		    float angleCurr = angle1;
		    float angleNext = angle1 + delta;

		    for (int i = 0; i < segments; i++)
	        {
		       Vector3 sphereCurr = new Vector3(
			       Mathf.Sin(Mathf.Deg2Rad * angleCurr), 
			       0,
			       Mathf.Cos(Mathf.Deg2Rad * angleCurr)
			   );

		       Vector3 sphereNext = new Vector3(
			       Mathf.Sin(Mathf.Deg2Rad * angleNext), 
			       0,
			       Mathf.Cos(Mathf.Deg2Rad * angleNext)
			   );

		       Vector3 posCurrMin = sphereCurr * minRadius;
		       Vector3 posCurrMax = sphereCurr * maxRadius;

		       Vector3 posNextMin = sphereNext * minRadius;
		       Vector3 posNextMax = sphereNext * maxRadius;

		       int a = 4 * i;
		       int b = 4 * i + 1;
		       int c = 4 * i + 2;
		       int d = 4 * i + 3;

		       vertices[a] = posCurrMin;
		       vertices[b] = posCurrMax;
		       vertices[c] = posNextMax;
		       vertices[d] = posNextMin;

		       triangles[6 * i] = a;
		       triangles[6 * i + 1] = b;
		       triangles[6 * i + 2] = c;
		       
		       triangles[6 * i + 3] = c;
		       triangles[6 * i + 4] = d;
		       triangles[6 * i + 5] = a;

		       angleCurr += delta;
		       angleNext += delta;
	        }

	        mesh.vertices = vertices;
	        mesh.triangles = triangles;

	        return mesh;
        }
	}
}