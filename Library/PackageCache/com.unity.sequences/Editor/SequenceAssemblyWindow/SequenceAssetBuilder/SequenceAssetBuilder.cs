using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Sequence Asset builders management.
    /// This is the main entry point to get the right builders from a given system Type.
    /// </summary>
    internal class SequenceAssetBuilder
    {
        /// <summary>
        /// Make builders available for use for specific types.
        /// </summary>
        static Dictionary<Type, ISequenceAssetBuilder> s_RegisteredBuilders = new Dictionary<Type, ISequenceAssetBuilder>()
        {
            { typeof(GameObject), new BasicSequenceAssetBuilder() }
        };

        /// <summary>
        /// Returns a registered builder for a given type.
        /// </summary>
        /// <param name="type">Type of the Sequence Asset.</param>
        /// <returns>Returns registered builder if one is found. Otherwise, returns BasicSequenceAssetBuilder.</returns>
        internal static ISequenceAssetBuilder GetBuilder(Type type)
        {
            if (!s_RegisteredBuilders.ContainsKey(type))
            {
                Debug.LogWarningFormat("SequenceAssetBuilder cannot find builder for type {0}", type);
                return s_RegisteredBuilders[typeof(GameObject)];
            }

            return s_RegisteredBuilders[type];
        }
    }
}
