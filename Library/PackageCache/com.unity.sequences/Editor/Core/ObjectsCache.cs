using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Sequences;
using UObject = UnityEngine.Object;
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Interface to help retrieve cached objects.
    /// Caches are invalidated at every Editor tick.
    /// </summary>
    static class ObjectsCache
    {
        /// <summary>
        /// Interface to implement for any caches that would be compatible with <see cref="ObjectsCacheSystem"/>
        /// </summary>
        interface ICache
        {
            IReadOnlyCollection<UObject> references { get; }

            bool shouldRebuild { get; }

            void InvalidateData();

            void Build();

            void RemoveInvalidReferences();
        }

        abstract class StageObjectsCache<T> : ICache where T : UObject
        {
            static List<GameObject> s_RootGameObjectsBuffer = new List<GameObject>();
            static List<T> s_ObjectBuffer = new List<T>();

            protected List<T> m_References = new List<T>();

            bool ICache.shouldRebuild => m_References.Count == 0;

            IReadOnlyCollection<UObject> ICache.references => m_References;

            public abstract IReadOnlyList<Scene> GetScenes();

            public void Build()
            {
                var scenes = GetScenes();
                for (int i = 0; i < scenes.Count; ++i)
                {
                    // Skip scenes that are visible but unloaded in the Hierarchy.
                    if (!scenes[i].isLoaded)
                        continue;

                    scenes[i].GetRootGameObjects(s_RootGameObjectsBuffer);
                    foreach (GameObject root in s_RootGameObjectsBuffer)
                    {
                        root.GetComponentsInChildren<T>(true, s_ObjectBuffer);
                        m_References.AddRange(s_ObjectBuffer);
                        s_ObjectBuffer.Clear();
                    }

                    s_RootGameObjectsBuffer.Clear();
                }
            }

            public virtual void InvalidateData()
            {
                m_References.Clear();
            }

            public virtual void RemoveInvalidReferences()
            {
                m_References.RemoveAll(obj => obj == null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ICache"/> for the Editor Main Stage.
        /// It holds references living in the Main Stage loaded scenes.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="UnityEngine.Object"/>.</typeparam>
        class MainStageObjectsCache<T> : StageObjectsCache<T> where T : UObject
        {
            List<Scene> m_SceneCache = new List<Scene>();

            public override IReadOnlyList<Scene> GetScenes()
            {
                m_SceneCache.Clear();

                for (int i = 0; i < EditorSceneManager.sceneCount; ++i)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    m_SceneCache.Add(scene);
                }

                return m_SceneCache;
            }
        }

        /// <summary>
        /// Implementation of <see cref="ICache"/> for the Editor Prefab Stage.
        /// It holds references living in the Prefab Stage loaded scenes.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="UnityEngine.Object"/>.</typeparam>
        class PrefabStageObjectsCache<T> : StageObjectsCache<T> where T : UObject
        {
            Scene[] m_SceneCache = new Scene[1];

            public override IReadOnlyList<Scene> GetScenes()
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                m_SceneCache[0] = stage.scene;

                return m_SceneCache;
            }
        }

        /// <summary>
        /// System holding the various caches.
        /// </summary>
        class ObjectsCacheSystem
        {
            enum CacheType
            {
                MainStage,
                PrefabStage
            }

            CacheType cacheType = CacheType.MainStage;
            Dictionary<Type, ICache> m_Caches = new Dictionary<Type, ICache>();

            internal IReadOnlyCollection<T> GetObjects<T>() where T : UObject
            {
                Type type = typeof(T);

                // Create cache for the given type if it does not exist.
                if (!m_Caches.ContainsKey(type))
                {
                    if (cacheType == CacheType.PrefabStage)
                        m_Caches.Add(type, new PrefabStageObjectsCache<T>());
                    else
                        m_Caches.Add(type, new MainStageObjectsCache<T>());
                }
                ICache cache = m_Caches[type];

                // Rebuild cache if it has been invalidated or is empty.
                if (cache.shouldRebuild)
                    cache.Build();
                else
                    cache.RemoveInvalidReferences();


                return cache.references as IReadOnlyCollection<T>;
            }

            internal void InvalidateCaches()
            {
                foreach (KeyValuePair<Type, ICache> cache in m_Caches)
                    cache.Value.InvalidateData();
            }

            internal void SwapToMainStageCache()
            {
                m_Caches.Clear();
                cacheType = CacheType.MainStage;
            }

            internal void SwapToPrefabStageCache()
            {
                m_Caches.Clear();
                cacheType = CacheType.PrefabStage;
            }
        }

        static ObjectsCacheSystem m_Internal;

        /// <summary>
        /// Initialize the internal cache system when the Editor loads.
        /// </summary>
        [InitializeOnLoadMethod]
        static void InitializeReferencesCache()
        {
            m_Internal = new ObjectsCacheSystem();

            EditorApplication.update += m_Internal.InvalidateCaches;

            // Listen to the following events in case data manipulation is happening from script in the same editor tick.
            SequenceUtility.sequenceCreated += OnSequenceCreated;
            SequenceUtility.sequenceDeleted += OnSequenceDeleted;
            SequenceAssetUtility.sequenceAssetAssignedTo += SequenceAssetUtility_sequenceAssetAssignedTo;
            SequenceAssetUtility.sequenceAssetRemovedFrom += SequenceAssetUtility_sequenceAssetRemovedFrom;
            EditorSceneManager.sceneLoaded += EditorSceneManager_sceneLoaded;
            EditorSceneManager.sceneUnloaded += EditorSceneManager_sceneUnloaded;
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            PrefabStage.prefabStageOpened += PrefabStage_prefabStageOpened;
            PrefabStage.prefabStageClosing += PrefabStage_prefabStageClosing;

            // Domain reload can happen while being in Prefab Stage.
            // This ensures we set the proper cache.
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                m_Internal.SwapToPrefabStageCache();
        }

        static void PrefabStage_prefabStageClosing(PrefabStage stage)
        {
            m_Internal.SwapToMainStageCache();
        }

        static void PrefabStage_prefabStageOpened(PrefabStage stage)
        {
            m_Internal.SwapToPrefabStageCache();
        }

        /// <summary>
        /// Returns a collection of references from matching Objects from loaded scenes.
        /// </summary>
        /// <typeparam name="T">Type of the requested objects. Must derived from <see cref="UnityEngine.Object" /></typeparam>
        /// <returns>A read only collection of objects found.</returns>
        internal static IReadOnlyCollection<T> FindObjectsFromScenes<T>() where T : UObject
        {
            return m_Internal.GetObjects<T>();
        }

        static void OnSequenceCreated(TimelineSequence sequence, MasterSequence masterSequence)
        {
            m_Internal.InvalidateCaches();
        }

        static void OnSequenceDeleted()
        {
            m_Internal.InvalidateCaches();
        }

        static void SequenceAssetUtility_sequenceAssetRemovedFrom(PlayableDirector obj)
        {
            m_Internal.InvalidateCaches();
        }

        static void SequenceAssetUtility_sequenceAssetAssignedTo(
            GameObject prefab,
            GameObject instance,
            PlayableDirector sequenceDirector)
        {
            m_Internal.InvalidateCaches();
        }

        static void EditorSceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            m_Internal.InvalidateCaches();
        }

        static void EditorSceneManager_sceneUnloaded(Scene scene)
        {
            m_Internal.InvalidateCaches();
        }

        static void EditorSceneManager_sceneOpened(Scene scene, OpenSceneMode mode)
        {
            m_Internal.InvalidateCaches();
        }
    }
}
