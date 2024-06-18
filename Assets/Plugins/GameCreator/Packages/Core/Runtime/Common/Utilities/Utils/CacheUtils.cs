using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    public static class CacheUtils
    {
        private class Cache
        {
            public readonly GameObject reference;
            public readonly Dictionary<Type, Component> components;

            public Cache(GameObject reference)
            {
                this.components = new Dictionary<Type, Component>();
                this.reference = reference;
            }
        }
        
        // VARIABLES: -----------------------------------------------------------------------------
        
        private static readonly Dictionary<int, Cache> CACHE = new Dictionary<int, Cache>();
        
        // INIT: ----------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemsInit()
        {
            CACHE.Clear();
            SceneManager.sceneUnloaded += scene => Prune();
        }
        
        // GET: -----------------------------------------------------------------------------------

        /// <summary>
        /// Returns the requested component (null if it does not exist) and caches its value
        /// so retrieving the same value afterwards is faster. 
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(this Component component) where T : Component
        {
            return Get(component, typeof(T)) as T;
        }
        
        /// <summary>
        /// Returns the requested component (null if it does not exist) and caches its value
        /// so retrieving the same value afterwards is faster. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(this GameObject gameObject) where T : Component
        {
            return Get(gameObject, typeof(T)) as T;
        }

        /// <summary>
        /// Returns the requested component (null if it does not exist) and caches its value
        /// so retrieving the same value afterwards is faster. 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component Get(this Component component, Type type)
        {
            return component != null ? Get(component.gameObject, type) : null;
        }
        
        /// <summary>
        /// Returns the requested component (null if it does not exist) and caches its value
        /// so retrieving the same value afterwards is faster. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component Get(this GameObject gameObject, Type type)
        {
            if (gameObject == null) return null;
            int instanceID = gameObject.GetInstanceID();

            if (!CACHE.TryGetValue(instanceID, out Cache cache))
            {
                cache = new Cache(gameObject);
                CACHE[instanceID] = cache;
            }

            if (!cache.components.TryGetValue(type, out Component component))
            {
                component = gameObject.GetComponent(type);
                if (component != null)
                {
                    cache.components[type] = component;
                    CACHE[instanceID] = cache;
                }
            }

            return component;
        }
        
        // ADD: -----------------------------------------------------------------------------------

        /// <summary>
        /// Adds a component and caches its value so retrieving the same value afterwards
        /// is faster.
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Add<T>(this Component component) where T : Component
        {
            return component != null ? Add<T>(component.gameObject) : null;
        }
        
        /// <summary>
        /// Adds a component and caches its value so retrieving the same value afterwards
        /// is faster.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Add<T>(this GameObject gameObject) where T : Component
        {
            return Add(gameObject, typeof(T)) as T;
        }

        /// <summary>
        /// Adds a component and caches its value so retrieving the same value afterwards
        /// is faster.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component Add(this Component component, Type type)
        {
            return component != null ? Add(component.gameObject, type) : null;
        }

        /// <summary>
        /// Adds a component and caches its value so retrieving the same value afterwards
        /// is faster.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component Add(this GameObject gameObject, Type type)
        {
            if (gameObject == null) return null;
            int instanceID = gameObject.GetInstanceID();

            if (!CACHE.TryGetValue(instanceID, out Cache cache))
            {
                cache = new Cache(gameObject);
                CACHE[instanceID] = cache;
            }

            Component component = gameObject.AddComponent(type);
            cache.components[type] = component;

            return component;
        }
        
        // PRUNE: ---------------------------------------------------------------------------------

        /// <summary>
        /// Iterates through the whole cache database and flushes all those elements which
        /// reference to null. This is an expensive operation and should only be done when the
        /// game is expected to be unresponsive, such as loading a scene. By default, when a scene
        /// is unloaded, this method will be executed.
        /// </summary>
        public static void Prune()
        {
            List<int> removeKeys = new List<int>();
            
            foreach (KeyValuePair<int,Cache> entry in CACHE)
            {
                if (entry.Value.reference == null)
                {
                    removeKeys.Add(entry.Key);
                }
            }

            foreach (int removeKey in removeKeys)
            {
                CACHE.Remove(removeKey);
            }
        }
    }
}