using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class QuaternionUtils
    {
        /// <summary>
        /// Clamps an angle between min and max values.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = Mathf.Repeat(angle, 360f);
            if (angle > 180f) angle -= 360f;
            
            min = Mathf.Repeat(min, 360f);
            max = Mathf.Repeat(max, 360f);
            
            if (min > 180f) min -= 360f;
            if (max > 180f) max -= 360f;
            
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// Returns the angular value between -180 and 180 degrees.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Convert180(float angle)
        {
            return Mathf.Repeat(angle + 180f, 360f) - 180f;
        }
        
        /// <summary>
        /// Rotates the current rotation towards the targeted one over time with a velocity and
        /// smooth time.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="smoothTime"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothDamp(Quaternion current, Quaternion target, 
            ref Quaternion velocity, float smoothTime, float deltaTime) 
        {
            if (deltaTime < Mathf.Epsilon) return current;
            
            float dot = Quaternion.Dot(current, target);
            float coefficient = dot > 0f ? 1f : -1f;
            
            target.x *= coefficient;
            target.y *= coefficient;
            target.z *= coefficient;
            target.w *= coefficient;
            
            Vector4 result = new Vector4(
                Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.z, target.z, ref velocity.z, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.w, target.w, ref velocity.w, smoothTime, Mathf.Infinity, deltaTime)
            ).normalized;
            
            Vector4 velocityError = Vector4.Project(
                new Vector4(velocity.x, velocity.y, velocity.z, velocity.w), 
                result
            );
            
            velocity.x -= velocityError.x;
            velocity.y -= velocityError.y;
            velocity.z -= velocityError.z;
            velocity.w -= velocityError.w;		
		
            return new Quaternion(result.x, result.y, result.z, result.w);
        }
    }
}