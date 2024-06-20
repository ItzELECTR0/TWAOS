using UnityEditor.Timeline;
using UnityEngine;

namespace UnityEditor.Sequences.Timeline
{
    // Pulled from com.unity.timeline.
    class TrackAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            // TODO: The EditorGUI.BeginChangeCheck is redundant with the serializedObject.Update. It can be removed.
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            // TODO: All this can be replace by `DrawPropertiesExcluding(serializedObject, "m_Script")`
            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                expanded = false;
                if (SkipField(property.propertyPath))
                    continue;
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        public virtual void ApplyChanges()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        static bool SkipField(string fieldName)
        {
            return fieldName == "m_Script";
        }
    }
}
