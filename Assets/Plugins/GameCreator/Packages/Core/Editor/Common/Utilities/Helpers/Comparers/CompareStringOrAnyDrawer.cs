using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(CompareStringOrAny))]
    public class CompareStringOrAnyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            SerializedProperty option = property.FindPropertyRelative("m_Option");
            SerializedProperty text = property.FindPropertyRelative("m_Text");
            
            PropertyField fieldOption = new PropertyField(option, property.displayName);
            
            PropertyElement fieldString = new PropertyElement(
                text.FindPropertyRelative(IPropertyDrawer.PROPERTY_NAME),
                string.Empty, true
            );
            
            head.Add(fieldOption);
            
            fieldOption.RegisterValueChangeCallback(changeEvent =>
            {
                body.Clear();
                if (changeEvent.changedProperty.intValue != 1) return;
                body.Add(fieldString);
                body.Bind(changeEvent.changedProperty.serializedObject);
            });

            if (option.intValue == 1)
            {
                body.Add(fieldString);
            }

            root.Add(head);
            root.Add(body);
            
            return root;
        }
    }
}
