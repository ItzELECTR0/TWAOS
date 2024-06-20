using UnityEngine;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("m_SceneAsset");

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                var selectedObject = EditorGUI.ObjectField(position, label, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);
                if (change.changed)
                    sceneAssetProperty.objectReferenceValue = selectedObject;
            }
        }
    }
}
