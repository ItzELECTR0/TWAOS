using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(IndexVariable))]
    public class IndexVariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty propertyValue = property.FindPropertyRelative("m_Value");
            PropertyField fieldValue = new PropertyField(propertyValue);
            
            root.Add(fieldValue);
            return root;
        }
    }
}