using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
    {
	    // STATIC FIELDS: -------------------------------------------------------------------------

        private static Mesh BOX_MESH;
    
        // PUBLIC METHODS: ------------------------------------------------------------------------
    
        public static void Box(Vector3 center, Quaternion rotation, Vector3 size)
        {
            Gizmos.DrawMesh(
                GetBoxMesh(),
                center, rotation, size
            );
        }
    
        public static void BoxWire(Vector3 center, Quaternion rotation, Vector3 size)
        {
	        Gizmos.DrawWireMesh(
                GetBoxMesh(),
		        center, rotation, size
	        );
        }
    
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Mesh GetBoxMesh()
        {
            if (BOX_MESH != null) return BOX_MESH;

            Vector3[] vertices = {
                new Vector3 (-0.5f, -0.5f, -0.5f),
                new Vector3 (+0.5f, -0.5f, -0.5f),
                new Vector3 (+0.5f, +0.5f, -0.5f),
                new Vector3 (-0.5f, +0.5f, -0.5f),
                new Vector3 (-0.5f, +0.5f, +0.5f),
                new Vector3 (+0.5f, +0.5f, +0.5f),
                new Vector3 (+0.5f, -0.5f, +0.5f),
                new Vector3 (-0.5f, -0.5f, +0.5f)
            };

            int[] triangles = {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5, 6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6
            };

            Mesh mesh = new Mesh
            {
                name = "Box",
                vertices = vertices, 
                triangles = triangles,
            };
            
            mesh.Optimize();
            mesh.RecalculateNormals();

            BOX_MESH = mesh;
            return mesh;
        }
    }
}