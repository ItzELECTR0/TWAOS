using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class QuaternionUtils
    {
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