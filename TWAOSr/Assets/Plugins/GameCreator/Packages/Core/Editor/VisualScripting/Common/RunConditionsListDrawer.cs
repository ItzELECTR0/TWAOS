using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(RunConditionsList))]
    public class RunConditionsListDrawer : PropertyDrawer
    {
        public const string PROP_CONDITIONS = "m_Conditions";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty conditions = property.FindPropertyRelative(PROP_CONDITIONS);
            return new PropertyField(conditions);
        }
    }
}