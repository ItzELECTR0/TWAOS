using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TChangeValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty operation = property.FindPropertyRelative("m_Operation");
            SerializedProperty value = property.FindPropertyRelative("m_Value");
                
            PropertyField fieldOperation = new PropertyField(operation, property.displayName);
            PropertyField fieldValue = new PropertyField(value, " ");

            root.Add(fieldOperation);
            root.Add(fieldValue);
            
            return root;
        }
    }
}