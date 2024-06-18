using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class ListUtils
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        /// <summary>
        /// Randomly shuffles an array
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 1; --i)
            {
                int j = Random.Range(0, i + 1);  
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}