using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
	{
		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public static void Vision(Vector3 position, Quaternion rotation, float angle, float radius, float height)
		{
			Mesh mesh = GetVisionMesh(angle, radius, height);

			Color color = Gizmos.color;
			Gizmos.DrawMesh(mesh, position, rotation, Vector3.one);
			Gizmos.color = color;
		}
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Mesh GetVisionMesh(float angle, float radius, float height)
        {
	        int segments = Mathf.CeilToInt(angle / 10f);
	        int verticesCount = (segments + 2) * 2;
	        int trianglesCount = segments * 3 * 2 + 12 + segments * 6;
	        
	        Mesh mesh = new Mesh
	        {
		        vertices = new Vector3[verticesCount],
		        triangles = new int[trianglesCount]
	        };
	     
	        Vector3[] normals = new Vector3[verticesCount];
	        Vector2[] uv = new Vector2[verticesCount];
	        
	        Vector3[] vertices = new Vector3[verticesCount];
	        int[] triangles = new int[trianglesCount];
	        
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
		    
		    float radians = Mathf.Deg2Rad * angle;
		    
		    int verticesOffset = segments + 2;
		    int trianglesOffset = segments * 3;
		    
		    vertices[0] = new Vector3(0f, -height * 0.5f, 0f);
		    vertices[verticesOffset] = new Vector3(0f, height * 0.5f, 0f);
		    
		    for (int i = 0; i <= segments; ++i)
		    {
			    float theta = i / (float) segments * radians;
			    
			    vertices[i + 1] = new Vector3(
				    Mathf.Sin(theta) * radius,
				    -height * 0.5f,
				    Mathf.Cos(theta) * radius
			    );
			    
			    vertices[verticesOffset + i + 1] = new Vector3(
				    Mathf.Sin(theta) * radius,
				    height * 0.5f,
				    Mathf.Cos(theta) * radius
			    );
			    
			    if (i == 0) continue;
			    
			    triangles[(i - 1) * 3 + 0] = 0;
			    triangles[(i - 1) * 3 + 1] = i + 1;
			    triangles[(i - 1) * 3 + 2] = i + 0;
			    
			    triangles[trianglesOffset + (i - 1) * 3 + 0] = verticesOffset;
			    triangles[trianglesOffset + (i - 1) * 3 + 1] = verticesOffset + i + 0;
			    triangles[trianglesOffset + (i - 1) * 3 + 2] = verticesOffset + i + 1;
		    }
		    
		    triangles[trianglesCount - segments * 6 - 12] = 0;
		    triangles[trianglesCount - segments * 6 - 11] = 1;
		    triangles[trianglesCount - segments * 6 - 10] = verticesOffset + 1;
		    
		    triangles[trianglesCount - segments * 6 - 9] = verticesOffset;
		    triangles[trianglesCount - segments * 6 - 8] = 0;
		    triangles[trianglesCount - segments * 6 - 7] = verticesOffset + 1;
		    
		    triangles[trianglesCount - segments * 6 - 6] = 0;
		    triangles[trianglesCount - segments * 6 - 5] = verticesOffset;
		    triangles[trianglesCount - segments * 6 - 4] = verticesOffset - 1;
		    
		    triangles[trianglesCount - segments * 6 - 3] = verticesOffset;
		    triangles[trianglesCount - segments * 6 - 2] = verticesOffset + verticesOffset - 1;
		    triangles[trianglesCount - segments * 6 - 1] = verticesOffset - 1;
		    
		    for (int i = 0; i < segments; ++i)
		    {
			    triangles[trianglesCount - (i * 6 + 1)] = i + 1;
			    triangles[trianglesCount - (i * 6 + 2)] = verticesOffset + i + 1;
			    triangles[trianglesCount - (i * 6 + 3)] = i + 2;

			    triangles[trianglesCount - (i * 6 + 4)] = i + 2;
			    triangles[trianglesCount - (i * 6 + 5)] = verticesOffset + i + 1;
			    triangles[trianglesCount - (i * 6 + 6)] = verticesOffset + i + 2;
		    }
		    
	        mesh.vertices = vertices;
	        mesh.triangles = triangles;
	        
	        return mesh;
        }
	}
}