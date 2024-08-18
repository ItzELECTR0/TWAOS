using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class Vector2Utils
    {
        /// <summary>
        /// Converts a Vector3 to a Vector2 using only its X and Y components
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 XY(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
        
        /// <summary>
        /// Converts a Vector3 to a Vector2 using only its X and Z components
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 XZ(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
    }
}