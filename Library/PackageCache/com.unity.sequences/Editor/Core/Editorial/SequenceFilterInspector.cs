using UnityEngine;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    [CustomEditor(typeof(SequenceFilter))]
    class SequenceFilterInspector : Editor
    {
        GameObject m_TargetGameObject;
        SerializedProperty m_MasterSequence;

        void OnEnable()
        {
            m_TargetGameObject = (target as SequenceFilter).gameObject;
            m_MasterSequence = serializedObject.FindProperty("m_MasterSequence");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(m_MasterSequence);
                EditorGUI.EndDisabledGroup();

                if (changeScope.changed)
                    serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Separator();

            using (new EditorGUILayout.VerticalScope())
            {
                AddComponentButton<SceneLoadingPolicy>(new GUIContent("Scene Loading Policy", "Helps define the loading policy for the scenes found in Timeline assets."), m_TargetGameObject);
            }
        }

        void AddComponentButton<TComponent>(GUIContent title, GameObject gameObjectTarget) where TComponent : Component
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = !gameObjectTarget.GetComponent<TComponent>();
                if (GUILayout.Button("Add Scene Loading Policy"))
                    Undo.AddComponent<TComponent>(gameObjectTarget);
                GUI.enabled = true;
            }
        }
    }
}
