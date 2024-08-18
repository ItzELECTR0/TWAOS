using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class CloneUtils
    {
        /// <summary>
        /// Makes a deep copy of a class or a struct. Note that this is an expensive method
        /// </summary>
        /// <param name="source">Instance to clone</param>
        /// <typeparam name="T">Type of the source and final instance</typeparam>
        /// <returns>Deep copy of the source instance</returns>
        public static T Deep<T>(T source)
        {
            if (source == null) return default;

            string jsonSource = JsonUtility.ToJson(source);
            T newInstance = JsonUtility.FromJson<T>(jsonSource);

            return newInstance;
        }
    }
}
