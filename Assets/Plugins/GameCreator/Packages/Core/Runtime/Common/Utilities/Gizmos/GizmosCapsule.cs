using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
    {
	    public enum CapsuleDirection
	    {
		    AxisX = 0,
		    AxisY = 1,
		    AxisZ = 2
	    }
	    
        private struct CapsuleData
        {
            private const float MEANINGFUL_DECIMALS = 1000f;

            // MEMBERS: ---------------------------------------------------------------------------
            
            private int m_Segments;
            private int m_Direction;
            
            private int m_Height;
            private int m_Radius;

            // PROPERTIES: ------------------------------------------------------------------------

            public int Segments => this.m_Segments;
            public int Direction => this.m_Direction;
            
            public float Height => this.m_Height / MEANINGFUL_DECIMALS;
            public float Radius => this.m_Radius / MEANINGFUL_DECIMALS;

            // CONSTRUCTOR: -----------------------------------------------------------------------

            public static CapsuleData Create(int segments, int direction, float height, float radius)
            {
                return new CapsuleData
                {
                    m_Segments = segments % 2 != 0 ? segments + 1 : segments,
                    m_Direction = Mathf.Clamp(direction, 0, 2),
                    m_Height = Mathf.FloorToInt(height * MEANINGFUL_DECIMALS),
                    m_Radius = Mathf.FloorToInt(radius * MEANINGFUL_DECIMALS)
                };
            }
        }
        
        private class CapsuleCache : Dictionary<CapsuleData, Mesh>
        { }
        
        // STATIC FIELDS: -------------------------------------------------------------------------
    
        private static readonly CapsuleCache CAPSULE_MESHES = new CapsuleCache();
    
        // PUBLIC METHODS: ------------------------------------------------------------------------
    
        public static void Capsule(Vector3 origin, Quaternion rotation,
            float radius, float height, int segments, int direction)
        {
            Gizmos.DrawMesh(
                RequestCapsule(radius, height, segments, direction),
                origin, rotation, Vector3.one
            );
        }
    
        public static void CapsuleWire(Vector3 origin, Quaternion rotation,
            float radius, float height, int segments, int direction)
        {
            Gizmos.DrawWireMesh(
                RequestCapsule(radius, height, segments, direction),
                origin, rotation, Vector3.one
            );
        }
    
        // MESH METHODS: --------------------------------------------------------------------------
    
        private static Mesh RequestCapsule(float radius, float height, int segments, int direction)
        {
            CapsuleData data = CapsuleData.Create(segments, direction, height, radius);
            if (!CAPSULE_MESHES.TryGetValue(data, out Mesh mesh) || mesh == null)
            {
                mesh = CreateCapsule(data);
                CAPSULE_MESHES[data] = mesh;
            }
            
            return mesh;
        }
    
        private static Mesh CreateCapsule(CapsuleData data)
        {
            int points = data.Segments + 1;
            
			float[] pX = new float[points];
			float[] pZ = new float[points];
			float[] pY = new float[points];
			float[] pR = new float[points];
			
			float calcH = 0f;
			float calcV = 0f;
			
			for (int i = 0; i < points; ++i)
			{
				pX[i] = Mathf.Sin(calcH * Mathf.Deg2Rad); 
				pZ[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
				pY[i] = Mathf.Cos(calcV * Mathf.Deg2Rad); 
				pR[i] = Mathf.Sin(calcV * Mathf.Deg2Rad); 
				
				calcH += 360f / data.Segments;
				calcV += 180f / data.Segments;
			}

			Vector3[] vertices = new Vector3[points * (points + 1)];
			Vector2[] uvs = new Vector2[vertices.Length];
			int index = 0;
			
			float yOff = (data.Height - (data.Radius * 2f)) * 0.5f;
			if (yOff < 0) yOff = 0;
			
			float stepX = 1f / (points - 1);
			
			float uvX;
			float uvY;
			
			int top = Mathf.CeilToInt(points * 0.5f);
			
			for (int y = 0; y < top; ++y) 
			{
				for (int x = 0; x < points; ++x) 
				{
					vertices[index] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * data.Radius;
					vertices[index].y = yOff + vertices[index].y;
					
					uvX = 1f - (stepX * x);
					uvY = (vertices[index].y + (data.Height * 0.5f)) / data.Height;
					uvs[index] = new Vector2(uvX, uvY);
					
					index++;
				}
			}
			
			int btm = Mathf.FloorToInt(points * 0.5f);
			
			for (int y = btm; y < points; ++y) 
			{
				for (int x = 0; x < points; ++x) 
				{
					vertices[index] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y] ) * data.Radius;
					vertices[index].y = -yOff + vertices[index].y;
					
					uvX = 1f - (stepX * x);
					uvY = (vertices[index].y + (data.Height * 0.5f)) / data.Height;
					uvs[index] = new Vector2(uvX, uvY);
					
					index++;
				}
			}

			int[] triangles = new int[data.Segments * (data.Segments + 1) * 2 * 3];
			
			for (int y = 0, t = 0; y < data.Segments + 1; ++y) 
			{
				for (int x = 0; x < data.Segments; ++x, t += 6) 
				{
					triangles[t + 0] = (y + 0) * (data.Segments + 1) + x + 0;
					triangles[t + 1] = (y + 1) * (data.Segments + 1) + x + 0;
					triangles[t + 2] = (y + 1) * (data.Segments + 1) + x + 1;
					
					triangles[t + 3] = (y + 0) * (data.Segments + 1) + x + 1;
					triangles[t + 4] = (y + 0) * (data.Segments + 1) + x + 0;
					triangles[t + 5] = (y + 1) * (data.Segments + 1) + x + 1;
				}
			}

			Quaternion rotation = data.Direction switch
			{
				0 => Quaternion.Euler(0f, 0f, 90f),
				1 => Quaternion.identity,
				2 => Quaternion.Euler(90f, 0f, 0f),
				_ => Quaternion.identity
			};
			
			for (int i = 0; i < vertices.Length; ++i)
			{
				vertices[i] = rotation * vertices[i];
			}

			Mesh mesh = new Mesh
            {
                name = "Capsule",
                vertices = vertices,
                uv = uvs,
                triangles = triangles
            };
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.Optimize();
    
            return mesh;
        }
    }
}