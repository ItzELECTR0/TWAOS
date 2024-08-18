using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static partial class GizmosExtension
	{
        private static readonly Vector3[] TRIANGLE_VERTICES =
        {
            new Vector3(-1, 0, 0),
            new Vector3( 0, 0, 1),
            new Vector3( 1, 0, 0),
            new Vector3(-1, 0, 0)
        };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Triangle(Vector3 position, Vector3 direction, float size = 1f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            Triangle(position, rotation, size);
        }

        public static void Triangle(Vector3 position, Quaternion rotation, float size = 1f)
        {
            for (int i = 1; i < TRIANGLE_VERTICES.Length; ++i)
            {
                Gizmos.DrawLine(
                    position + (rotation * TRIANGLE_VERTICES[i - 0] * size),
                    position + (rotation * TRIANGLE_VERTICES[i - 1] * size)
                );
            }
        }
    }
}