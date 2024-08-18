using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(CompareGameObjectOrAny))]
    public class CompareGameObjectOrAnyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            SerializedProperty option = property.FindPropertyRelative("m_Option");
            SerializedProperty gameObject = property.FindPropertyRelative("m_GameObject");
            
            PropertyField fieldOption = new PropertyField(option, property.displayName);
            
            PropertyElement fieldGameObject = new PropertyElement(
                gameObject.FindPropertyRelative(IPropertyDrawer.PROPERTY_NAME),
                string.Empty, true
            );
            
            head.Add(fieldOption);
            
            fieldOption.RegisterValueChangeCallback(changeEvent =>
            {
                body.Clear();
                if (changeEvent.changedProperty.intValue != 1) return;
                body.Add(fieldGameObject);
                body.Bind(changeEvent.changedProperty.serializedObject);
            });

            if (option.intValue == 1)
            {
                body.Add(fieldGameObject);
            }

            root.Add(head);
            root.Add(body);
            
            return root;
        }
    }
}
