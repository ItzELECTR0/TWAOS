using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(TValue), true)]
    public class TValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("m_Value");
            if (value == null) return new VisualElement();
            
            PropertyField field = new PropertyField(value, this.preferredLabel);
            field.Bind(property.serializedObject);
            
            return field;
        }
    }
}