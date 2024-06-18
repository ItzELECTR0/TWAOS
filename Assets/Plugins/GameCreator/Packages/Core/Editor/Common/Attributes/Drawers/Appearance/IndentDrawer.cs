using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentPD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IndentAttribute indent = this.attribute as IndentAttribute;
            int indentValue = indent?.Level ?? 0;
            
            EditorGUI.indentLevel += indentValue;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel -= indentValue;
        }
    }
}