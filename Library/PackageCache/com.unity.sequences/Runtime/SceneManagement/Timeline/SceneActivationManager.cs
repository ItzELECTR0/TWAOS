using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.Sequences
{
    /// <summary>
    /// Manager used by SceneActivationTrack and SceneActivationMixer.
    /// It enables/disables Scenes based on the number of requests it receives.
    /// </summary>
    static class SceneActivationManager
    {
        /// <summary>
        /// List of active tracked Scenes.
        /// </summary>
        static readonly List<SceneTracker> s_SceneTrackers = new List<SceneTracker>();

        /// <summary>
        /// Activation strategy to apply when enabling or disabling a Scene.
        /// </summary>
        static readonly ISceneActivationBehaviour s_ActivationStrategy = new BasicSceneActivation();

        /// <summary>
        /// Tracker object used to follow the number of requesters (= SceneActivationTrack) for a given Scene.
        /// </summary>
        /// <remarks>
        /// Mostly useful when multiple SceneActivationTrack request the same Scene.
        /// A weight is computed based on the received request to ensure if a given Scene must be enabled.
        /// </remarks>
        [System.Serializable]
        class SceneTracker
        {
            SceneReference m_Scene;

            /// <summary>
            /// Scene path relative to the Assets/ folder.
            /// </summary>
            public string path
            {
                get => m_Scene.path;
                set => m_Scene = new SceneReference() {path = value};
            }

            /// <summary>
            /// List of request received for a given frame.
            /// </summary>
            List<bool> m_RequestReceivedCount = new List<bool>();

            /// <summary>
            /// Number of registered requester.
            /// </summary>
            int m_ReferenceCount = 0;

            /// <summary>
            ///  Number of requester about to be deleted>
            /// </summary>
            int m_ReferenceTrashCount;

            public bool hasReceivedOneActiveRequest => m_RequestReceivedCount.Contains(true);

            public void RequestState(bool state)
            {
                m_RequestReceivedCount.Add(state);
            }

            public void IncrementRegisteredRequesterCount()
            {
                ++m_ReferenceCount;
            }

            void DecrementRegisterRequesterCount()
            {
                --m_ReferenceCount;
            }

            public void RemoveRequester()
            {
                ++m_ReferenceTrashCount;
                DecrementRegisterRequesterCount();
            }

            public bool HasRegisteredRequester()
            {
                return m_ReferenceCount > 0;
            }

            /// <summary>
            /// Indicates if the tracker has received a request from every registered requester.
            /// </summary>
            /// <returns>False when not all requester have made their request yet.</returns>
            public bool Completed()
            {
                return m_ReferenceCount + m_ReferenceTrashCount == m_RequestReceivedCount.Count;
            }

            /// <summary>
            /// Reset the current list of received state requests and references to be deleted.
            /// Call this method at the beginning of a new frame.
            /// </summary>
            public void Reset()
            {
                m_RequestReceivedCount.Clear();
                m_ReferenceTrashCount = 0;
            }
        }

        internal static Scene GetScene(string path)
        {
            return SceneManager.GetSceneByPath(path);
        }

        /// <summary>
        /// Requests a Scene to be activated.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        internal static void RequestActivateScene(string path)
        {
            Scene loadedScene = SceneManager.GetSceneByPath(path);

            if (!loadedScene.isLoaded)
                return;

            SceneTracker tracker = GetSceneTracker(path);
            tracker.RequestState(true);

            if (tracker.Completed())
                ProcessState(tracker);
        }

        /// <summary>
        /// Requests a Scene to be deactivated.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        internal static void RequestDeactivateScene(string path)
        {
            Scene loadedScene = SceneManager.GetSceneByPath(path);
            if (!loadedScene.isLoaded)
                return;

            SceneTracker tracker = GetSceneTracker(path);
            tracker.RequestState(false);

            if (tracker.Completed())
                ProcessState(tracker);
        }

        /// <summary>
        /// Registers a requester. This must be done before making a request.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        internal static void Register(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new System.ArgumentException("Invalid scene path provided.");

            var tracker = GetSceneTracker(path);
            if (tracker != null)
            {
                tracker.IncrementRegisteredRequesterCount();
                return;
            }

            tracker = new SceneTracker() { path = path };
            tracker.IncrementRegisteredRequesterCount();
            s_SceneTrackers.Add(tracker);
        }

        /// <summary>
        /// Unregisters a requester.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        internal static void Unregister(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new System.ArgumentException("Invalid scene path provided.");

            var tracker = GetSceneTracker(path);
            if (tracker == null)
                return;

            RequestDeactivateScene(path);

            tracker.RemoveRequester();
            if (!tracker.HasRegisteredRequester())
                s_SceneTrackers.Remove(tracker);
        }

        /// <summary>
        /// Processes the received requests for a frame then executes the active activation strategy.
        /// </summary>
        /// <param name="tracker"></param>
        static void ProcessState(SceneTracker tracker)
        {
            Scene loadedScene = GetScene(tracker.path);

            if (!loadedScene.isLoaded)
                return;

            s_ActivationStrategy.Execute(loadedScene, tracker.hasReceivedOneActiveRequest);
            tracker.Reset();
        }

        /// <summary>
        /// Gets the SceneTracker for the given Scene path.
        /// </summary>
        /// <param name="path">The Scene path to use to find the corresponding SceneTracker.</param>
        /// <returns>The SceneTracker found, or null if no SceneTracker exists for the given Scene path.</returns>
        static SceneTracker GetSceneTracker(string path)
        {
            SceneTracker tracker = s_SceneTrackers.Find(sceneTracker => sceneTracker.path == path);
            return tracker;
        }
    }
}
