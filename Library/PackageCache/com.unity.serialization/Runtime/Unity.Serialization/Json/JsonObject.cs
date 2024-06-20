using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// The default object output by <see cref="JsonSerialization"/> if an object type can not be resolved.
    /// </summary>
    public class JsonObject : Dictionary<string, object>
    {
        private static bool k_Registered = false;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]  
        static void RegisterPropertyBag()
        {
            if (k_Registered) return;

            k_Registered = true;
            PropertyBag.Register(new KeyValueCollectionPropertyBag<JsonObject, string, object>());
        }
    }

    /// <summary>
    /// The default object output by <see cref="JsonSerialization"/> if an array type can not be resolved.
    /// </summary>
    public class JsonArray : List<object>
    {
        private static bool k_Registered = false;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]  
        static void RegisterPropertyBag()
        {
            if (k_Registered) return;

            k_Registered = true;
            PropertyBag.Register(new IndexedCollectionPropertyBag<JsonArray, object>());
        }
    }
}