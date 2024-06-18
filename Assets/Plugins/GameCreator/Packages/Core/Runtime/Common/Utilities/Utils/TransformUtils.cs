using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class TransformUtils
    {
        /// <summary>
        /// Converts a point from local to world space, without taking into account the
        /// scale of the Transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 TransformPointUnscale(Transform transform, Vector3 point)
        {
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                Vector3.one
            );
            
            return localToWorldMatrix.MultiplyPoint3x4(point);
        }
        
        /// <summary>
        /// Converts a point from world space to local space, without taking into account the
        /// scale of the Transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 InverseTransformPointUnscale(Transform transform, Vector3 point)
        {
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(
                transform.position, 
                transform.rotation,
                Vector3.one
            );
            
            return localToWorldMatrix.inverse.MultiplyPoint3x4(point);
        }
    }
}