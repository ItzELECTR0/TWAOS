using UnityEditor;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ConditionDisablePlaymodeAttribute))]
    public class ConditionDisablePlaymodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}