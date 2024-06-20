#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UnityEngine.Sequences
{
    /// <summary>
    /// Scene wrapper to allow proper serialization of SceneAsset references.
    /// SceneAsset only exist in Editor so the path is stored as well any time the object is serialized.
    /// The Scene path is used at runtime, assuming the Scene has been added to the Build Settings.
    /// </summary>
    // Inspired from: https://github.com/starikcetin/unity-scene-reference
    [System.Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField] Object m_SceneAsset;

        bool isValidSceneAsset
        {
            get
            {
                if (!m_SceneAsset) return false;

                return m_SceneAsset is SceneAsset;
            }
        }
#endif

        [SerializeField][HideInInspector] string m_ScenePath;

        /// <summary>
        /// Path to the Scene asset, relative to the project folder.
        /// </summary>
        public string path
        {
            get
            {
#if UNITY_EDITOR
                return GetScenePathFromAsset();
#else
                return m_ScenePath;
#endif
            }
            set
            {
                m_ScenePath = value;
#if UNITY_EDITOR
                m_SceneAsset = GetSceneAssetFromPath();
#endif
            }
        }

#if UNITY_EDITOR
        Object GetSceneAssetFromPath()
        {
            return string.IsNullOrEmpty(m_ScenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(m_ScenePath);
        }

        string GetScenePathFromAsset()
        {
            return m_SceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(m_SceneAsset);
        }

        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference.path;
        }

        void BeforeSerialize()
        {
            // Asset is invalid but have a path try to get the Scene
            if (isValidSceneAsset == false && string.IsNullOrEmpty(m_ScenePath) == false)
            {
                m_SceneAsset = GetSceneAssetFromPath();
                if (m_SceneAsset == null) m_ScenePath = string.Empty;

                EditorSceneManager.MarkAllScenesDirty();
            }
            // Asset takes precedence and overwrites path
            else
            {
                m_ScenePath = GetScenePathFromAsset();
            }
        }

        void AfterDeserialize()
        {
            EditorApplication.update -= AfterDeserialize;
            BeforeSerialize();
        }

#endif
        /// <summary>
        /// Receives callback event from <see cref="ISerializationCallbackReceiver"/>.
        /// </summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            BeforeSerialize();
#endif
        }

        /// <summary>
        /// Receives callback event from <see cref="ISerializationCallbackReceiver"/>.
        /// </summary>
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // The AssetDatabase can't be accessed during serialization, delay the call.
            EditorApplication.update += AfterDeserialize;
#endif
        }
    }
}
