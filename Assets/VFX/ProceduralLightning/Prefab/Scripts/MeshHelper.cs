//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections.Generic;

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Mesh helper methods
    /// </summary>
    public class MeshHelper
    {
        private Mesh mesh;
        private int[] triangles;
        private Vector3[] vertices;
        private Vector3[] normals;
        private float[] normalizedAreaWeights;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mesh">Mesh</param>
        public MeshHelper(Mesh mesh)
        {
            this.mesh = mesh;
            this.triangles = mesh.triangles;
            this.vertices = mesh.vertices;
            this.normals = mesh.normals;            
            CalculateNormalizedAreaWeights();
        }

        /// <summary>
        /// Get random point on mesh
        /// </summary>
        /// <param name="hit">Raycast</param>
        /// <param name="triangleIndex">Received indice</param>
        public void GenerateRandomPoint(ref RaycastHit hit, out int triangleIndex)
        {
            triangleIndex = SelectRandomTriangle();
            GetRaycastFromTriangleIndex(triangleIndex, ref hit);
        }

        /// <summary>
        /// Get ray dir from indice
        /// </summary>
        /// <param name="triangleIndex">Indice</param>
        /// <param name="hit">Raycast result</param>
        public void GetRaycastFromTriangleIndex(int triangleIndex, ref RaycastHit hit)
        {
            Vector3 bc = GenerateRandomBarycentricCoordinates();
            Vector3 p1 = vertices[triangles[triangleIndex]];
            Vector3 p2 = vertices[triangles[triangleIndex + 1]];
            Vector3 p3 = vertices[triangles[triangleIndex + 2]];

            hit.barycentricCoordinate = bc;
            hit.point = ((p1 * bc.x) + (p2 * bc.y) + (p3 * bc.z));

            if (normals == null)
            {
                // face normal
                hit.normal = Vector3.Cross((p3 - p2), (p1 - p2)).normalized;
            }
            else
            {
                // interpolated vertex normal
                p1 = normals[triangles[triangleIndex]];
                p2 = normals[triangles[triangleIndex + 1]];
                p3 = normals[triangles[triangleIndex + 2]];
                hit.normal = (p1 * bc.x) + (p2 * bc.y) + (p3 * bc.z);
            }
        }

        /// <summary>
        /// Mesh
        /// </summary>
        public Mesh Mesh
        {
            get { return mesh; }
        }

        /// <summary>
        /// Indices
        /// </summary>
        public int[] Triangles
        {
            get { return triangles; }
        }

        /// <summary>
        /// Vertices
        /// </summary>
        public Vector3[] Vertices
        {
            get { return vertices; }
        }

        /// <summary>
        /// Normals
        /// </summary>
        public Vector3[] Normals
        {
            get { return normals; }
        }

        private float[] CalculateSurfaceAreas(out float totalSurfaceArea)
        {
            int idx = 0;
            totalSurfaceArea = 0.0f;
            float[] surfaceAreas = new float[triangles.Length / 3];
            for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3)
            {
                Vector3 p1 = vertices[triangles[triangleIndex]];
                Vector3 p2 = vertices[triangles[triangleIndex + 1]];
                Vector3 p3 = vertices[triangles[triangleIndex + 2]];

                // http://www.wikihow.com/Sample/Area-of-a-Triangle-Side-Length
                float a = (p1 - p2).sqrMagnitude;
                float b = (p1 - p3).sqrMagnitude;
                float c = (p2 - p3).sqrMagnitude;

                // faster with only 1 square root: http://www.iquilezles.org/blog/?p=1579
                // A² = (2ab + 2bc + 2ca – a² – b² – c²)/16
                float areaSquared = ((2.0f * a * b) + (2.0f * b * c) + (2.0f * c * a) - (a * a) - (b * b) - (c * c)) / 16.0f;
                float area = PathGenerator.SquareRoot(areaSquared);
                surfaceAreas[idx++] = area;
                totalSurfaceArea += area;
            }

            return surfaceAreas;
        }

        private void CalculateNormalizedAreaWeights()
        {
            // create a sorted array of normalized area weights - this is an aggregate and is easily binary searched with a random value between 0 and 1 to find
            // a random triangle. Larger triangles have bigger gaps in the array.
            float totalSurfaceArea;
            normalizedAreaWeights = CalculateSurfaceAreas(out totalSurfaceArea);
            if (normalizedAreaWeights.Length == 0)
            {
                return;
            }
            float normalizedArea;
            float normalizedAggregate = 0.0f;
            for (int i = 0; i < normalizedAreaWeights.Length; i++)
            {
                normalizedArea = normalizedAreaWeights[i] / totalSurfaceArea;
                normalizedAreaWeights[i] = normalizedAggregate;
                normalizedAggregate += normalizedArea;
            }
        }

        private int SelectRandomTriangle()
        {
            float randomValue = Random.value;
            int imin = 0;
            int imax = normalizedAreaWeights.Length - 1;
            while (imin < imax)
            {
                int imid = (imin + imax) / 2;
                if (normalizedAreaWeights[imid] < randomValue)
                {
                    imin = imid + 1;
                }
                else
                {
                    imax = imid;
                }
            }
            return imin * 3;
        }

        private Vector3 GenerateRandomBarycentricCoordinates()
        {
            Vector3 barycentric = new Vector3(Random.Range(Mathf.Epsilon, 1.0f), Random.Range(Mathf.Epsilon, 1.0f), Random.Range(Mathf.Epsilon, 1.0f));

            // normalize the barycentric coordinates. These are normalized such that x + y + z = 1, as opposed to
            // normal vectors which are normalized such that Sqrt(x^2 + y^2 + z^2) = 1. See:
            // http://en.wikipedia.org/wiki/Barycentric_coordinate_system

            return barycentric / (barycentric.x + barycentric.y + barycentric.z);
        }
    }
}
