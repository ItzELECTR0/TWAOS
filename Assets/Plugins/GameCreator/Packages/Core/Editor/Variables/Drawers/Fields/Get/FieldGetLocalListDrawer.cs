using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(FieldGetLocalList))]
    public class FieldGetLocalListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty propertyVariable = property.FindPropertyRelative("m_Variable");
            SerializedProperty propertySelect = property.FindPropertyRelative("m_Select");

            PropertyField fieldVariable = new PropertyField(propertyVariable);
            PropertyElement fieldSelect = new PropertyElement(propertySelect, propertySelect.displayName, false);
            
            root.Add(fieldVariable);
            root.Add(fieldSelect);

            return root;
        }
    }
}