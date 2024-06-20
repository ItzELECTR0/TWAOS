using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UnityEngine.Sequences
{
    [RequireComponent(typeof(SequenceFilter))]
    [DisallowMultipleComponent()]
    [ComponentHelpURL("scene-loading-policy")]
    class SceneLoadingPolicy : MonoBehaviour
    {
        internal enum Policy
        {
            DoNotLoadUnloadedScenes,
            LoadAllUnloadedScenes
        }

        internal enum ExecutionDomain
        {
            PlayModeOnly,
            PlayerBuildOnly,
            PlayModeAndPlayerBuild
        }

        [Tooltip("The Scene loading policy to apply to any unloaded Scene of the current Master Sequence.")]
        [SerializeField]
        Policy m_ActivePolicy = Policy.LoadAllUnloadedScenes;

        [Tooltip("The runtime mode(s) to apply the policy to: Play mode, Player build, or both.")]
        [SerializeField]
        [FormerlySerializedAs("m_Domain")]
        ExecutionDomain m_RuntimeMode = ExecutionDomain.PlayModeAndPlayerBuild;

        internal Policy activePolicy
        {
            get => m_ActivePolicy;
            set => m_ActivePolicy = value;
        }

        internal ExecutionDomain runtimeMode
        {
            get => m_RuntimeMode;
            set => m_RuntimeMode = value;
        }

        internal void Awake()
        {
            if (ShouldExecute())
                ApplyLoadPolicy();
        }

        bool ShouldExecute()
        {
            return (m_RuntimeMode == ExecutionDomain.PlayModeAndPlayerBuild)
                || (m_RuntimeMode == ExecutionDomain.PlayModeOnly && Application.isEditor)
                || (m_RuntimeMode == ExecutionDomain.PlayerBuildOnly && !Application.isEditor);
        }

        void ApplyLoadPolicy()
        {
            switch (m_ActivePolicy)
            {
                case Policy.LoadAllUnloadedScenes:
                    ApplyLoadAllScenesPolicy();
                    break;
                case Policy.DoNotLoadUnloadedScenes:
                default:
                    break;
            }
        }

        void ApplyLoadAllScenesPolicy()
        {
            SequenceFilter filter = gameObject.GetComponent<SequenceFilter>();
            if (filter.masterSequence == null)
                return;

            IReadOnlyCollection<string> paths = filter.masterSequence.rootSequence.GetRelatedScenes();

            foreach (string scenePath in paths)
                LoadScene(scenePath);
        }

        // Load scenes not in async as the Recorder won't wait for them before recording frames.
        void LoadScene(string scenePath)
        {
#if UNITY_EDITOR
            if (EditorSceneManager.GetSceneByPath(scenePath) == default)
                EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));
#else
            if (SceneManager.GetSceneByPath(scenePath) == default)
                SceneManager.LoadScene(scenePath, SceneManagement.LoadSceneMode.Additive);
#endif
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (m_RuntimeMode == ExecutionDomain.PlayModeOnly)
                return;

            var sequenceFilter = GetComponent<SequenceFilter>();
            if (sequenceFilter.masterSequence == null ||
                TimelineSequence.IsNullOrEmpty(sequenceFilter.masterSequence.rootSequence))
            {
                return;
            }

            // Add scene to Build settings.
            var scenes = EditorBuildSettings.scenes.ToList();
            var newScenesPath = sequenceFilter.masterSequence.rootSequence.GetRelatedScenes().ToList();

            // In the case of creating a Prefab with a GameObject that has this component, the scene is null.
            if (gameObject.scene != default)
                newScenesPath.Insert(0, gameObject.scene.path);

            foreach (var scenePath in newScenesPath)
            {
                var buildSettingsScene = scenes.Find(settings => settings.path == scenePath);

                if (buildSettingsScene != null)
                {
                    buildSettingsScene.enabled = true;
                    continue;
                }

                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

#endif
    }
}
