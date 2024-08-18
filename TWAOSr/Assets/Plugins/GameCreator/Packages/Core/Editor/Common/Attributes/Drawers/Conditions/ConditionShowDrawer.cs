using UnityEditor;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ConditionShowAttribute))]
    public class ConditionShowDrawer : ConditionBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!this.FieldValue(property)) return;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!this.FieldValue(property)) return 0f;
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}