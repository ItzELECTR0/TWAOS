using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(NameVariable))]
    public class NameVariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty propertyName = property.FindPropertyRelative("m_Name");
            SerializedProperty propertyValue = property.FindPropertyRelative("m_Value");

            PropertyField fieldName = new PropertyField(propertyName);
            PropertyElement fieldValue = new PropertyElement(
                propertyValue,
                propertyValue.displayName,
                true
            );

            root.Add(fieldName);
            root.Add(new SpaceSmaller());
            root.Add(fieldValue);

            return root;
        }
    }
}