using System;
using System.Collections.Generic;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaimahouGames.Runtime.Core
{
    public static class FeatureCache
    {
        //============================================================================================================||
        // -----------------------------------------------------------------------------------------------------------|
        
        private struct Cache
        {
            public Pawn Reference { get; }
            public Dictionary<Type, Feature> Features { get; }

            public Cache(Pawn reference)
            {
                Reference = reference;
                Features = new Dictionary<Type, Feature>();
            }
        }
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        private static readonly Dictionary<int, Cache> CACHE = new();
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public static T Get<T>(this Feature feature) where T : Component
        {
            return feature.GameObject.Get<T>();
        }
        
        public static T Get<T>(this PropertyGetGameObject go, Args args) where T : Feature
        {
            return go.Get<Pawn>(args).Get<T>();
        }

        public static T Get<T>(this GameObject go) where T : Feature => go.Get<Pawn>().Get<T>();
        public static T Get<T>(this Pawn pawn) where T : Feature => Get(pawn, typeof(T)) as T;

        public static Feature Get(this Pawn pawn, Type type)
        {
            if (pawn == null) return null;
            var instanceID = pawn.GetInstanceID();

            if (!CACHE.TryGetValue(instanceID, out var cache))
            {
                cache = new Cache(pawn);
                CACHE[instanceID] = cache;
            }

            if (!cache.Features.TryGetValue(type, out var feature))
            {
                feature = pawn.GetFeature(type);
                if (feature != null)
                {
                    cache.Features[type] = feature;
                    CACHE[instanceID] = cache;
                }
            }

            return feature;
        }

        /// <summary>
        /// Iterates through the whole cache database and flushes all those elements which
        /// reference to null. This is an expensive operation and should only be done when the
        /// game is expected to be unresponsive, such as loading a scene. By default, when a scene
        /// is unloaded, this method will be executed.
        /// </summary>
        public static void Prune()
        {
            var removeKeys = new List<int>();
            
            foreach (var (key, value) in CACHE)
            {
                if (value.Reference == null) removeKeys.Add(key);
            }

            foreach (var removeKey in removeKeys) CACHE.Remove(removeKey);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemsInit()
        {
            CACHE.Clear();
            SceneManager.sceneUnloaded += scene => Prune();
        }
        
        //============================================================================================================||
    }
}