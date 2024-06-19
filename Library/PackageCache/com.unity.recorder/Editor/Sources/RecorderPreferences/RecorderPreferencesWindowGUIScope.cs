using UnityEngine;
using UnityEditor;

namespace UnityEditor.Recorder
{
    class RecorderPreferencesWindowGUIScope : GUI.Scope
    {
        const float k_DefaultLabelWidth = 250f;
        const float k_DefaultLayoutMaxWidth = 500f;
        const float k_MarginLeft = 10f;
        const float k_MarginTop = 10f;

        float m_LabelWidth;

        public RecorderPreferencesWindowGUIScope(float layoutMaxWidth)
        {
            m_LabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = k_DefaultLabelWidth;
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_MarginLeft);
            GUILayout.BeginVertical();
            GUILayout.Space(k_MarginTop);
        }

        public RecorderPreferencesWindowGUIScope() : this(k_DefaultLayoutMaxWidth)
        {
        }

        protected override void CloseScope()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = m_LabelWidth;
        }
    }
}
