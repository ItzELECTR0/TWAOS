using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static partial class GizmosExtension
	{
        public static void Cylinder(Vector3 origin, float height, float radius)
        {
            Cylinder(origin, origin + Vector3.up * height, radius);
        }

        public static void Cylinder(Vector3 positionA, Vector3 positionB, float radius)
        {
            Vector3 top = (positionB - positionA).normalized * radius;
            Vector3 fwd = Vector3.Slerp(top, -top, 0.5f);
            Vector3 rht = Vector3.Cross(top, fwd).normalized * radius;

            Circle(positionA, radius, top);
            Circle(positionB, radius, -top);
            
            Gizmos.DrawLine(positionA + rht, positionB + rht);
            Gizmos.DrawLine(positionA - rht, positionB - rht);

            Gizmos.DrawLine(positionA + fwd, positionB + fwd);
            Gizmos.DrawLine(positionA - fwd, positionB - fwd);
        }
    }
}