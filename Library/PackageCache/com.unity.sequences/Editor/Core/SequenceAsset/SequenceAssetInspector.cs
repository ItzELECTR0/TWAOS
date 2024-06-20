using System;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    // Todo: This class should be made private: https://jira.unity3d.com/browse/SEQ-542
    // Note: The documentation XML are added only to remove warning when validating the package until this class
    //       can be made private. In the meantime, it is explicitly excluded from the documentation, see
    //       Documentation > filter.yml

    /// <summary>
    ///
    /// </summary>
    [CustomEditor(typeof(SequenceAsset))]
    class SequenceAssetInspector : Editor
    {
        SerializedProperty m_Type;

        void OnEnable()
        {
            m_Type = serializedObject.FindProperty("m_Type");
        }

        /// <summary>
        ///
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var options = CollectionType.instance.GetTypes();
            var selected = Array.IndexOf(options, m_Type.stringValue);
            selected = EditorGUILayout.Popup("Type", selected, options);
            m_Type.stringValue = options[selected];

            serializedObject.ApplyModifiedProperties();
        }
    }
}
