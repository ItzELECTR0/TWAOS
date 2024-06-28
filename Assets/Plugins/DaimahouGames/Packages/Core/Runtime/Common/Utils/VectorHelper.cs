using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class VectorHelper
    {
        public static float Distance2D(Vector3 v1, Vector3 v2) => Vector3.Distance(
            Vector3.ProjectOnPlane(v1, Vector3.up),
            Vector3.ProjectOnPlane(v2, Vector3.up)
        );

        public static float Angle2D(Transform casterTransform, Vector3 targetPosition)
        {
            var targetDir = Vector3.ProjectOnPlane(
                targetPosition - casterTransform.position, 
                casterTransform.up);
            
            return Vector3.Angle(targetDir, casterTransform.forward);
        }
    }
}