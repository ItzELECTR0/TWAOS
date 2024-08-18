using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class Vector3Utils
    {
        /// <summary>
        /// Projects a point onto a segment and returns a point between A and B that is closest to
        /// the Point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns></returns>
        public static Vector3 OnSegment(this Vector3 point, Vector3 pointA, Vector3 pointB)
        {
            Vector3 direction = pointB - pointA;
            
            float length = direction.magnitude;
            if (length < float.Epsilon) return pointA;
            
            direction.Normalize();
   
            Vector3 directionPA = point - pointA;
            float projection = Vector3.Dot(directionPA, direction);
            
            projection = Mathf.Clamp(projection, 0f, length);
            return pointA + direction * projection;
        }

        /// <summary>
        /// Returns the projection of a point onto a vector defined by the given direction.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 PointOnVector(this Vector3 point, Vector3 direction)
        {
            float scalarProjection = Vector3.Dot(point, direction) / direction.magnitude;
            return scalarProjection * direction;
        }
    }
}