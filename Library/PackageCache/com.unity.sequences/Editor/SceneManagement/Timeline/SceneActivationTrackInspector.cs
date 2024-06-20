using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    [CustomEditor(typeof(SceneActivationTrack))]
    class SceneActivationTrackInspector : TrackAssetInspector
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            var sceneProperty = serializedObject.FindProperty("scene");
            var sceneRef = sceneProperty.FindPropertyRelative("m_SceneAsset");

            using (new GUILayout.HorizontalScope())
            {
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.ObjectField(sceneRef, typeof(SceneAsset));
                    if (change.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    }
                }

                SerializedProperty sceneAssetProperty = sceneProperty.FindPropertyRelative("m_SceneAsset");
                var scenePath = sceneAssetProperty.objectReferenceValue == null ?
                    string.Empty :
                    AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue);

                using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(scenePath) || SceneManagement.IsLoaded(scenePath)))
                {
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(60)))
                    {
                        SceneManagement.OpenScene(scenePath);
                        TimelineEditor.Refresh(RefreshReason.SceneNeedsUpdate);
                    }
                }

                using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(scenePath) || !SceneManagement.IsLoaded(scenePath)))
                {
                    if (GUILayout.Button("Unload", GUILayout.MaxWidth(60)))
                        SceneManagement.CloseScene(scenePath);
                }
            }
        }
    }
}
