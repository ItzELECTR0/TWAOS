using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(CompareMinDistanceOrNone))]
    public class CompareMinDistanceOrNoneDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            SerializedProperty option = property.FindPropertyRelative("m_MinDistance");
            SerializedProperty target = property.FindPropertyRelative("m_To");
            SerializedProperty radius = property.FindPropertyRelative("m_Radius");
            SerializedProperty offset = property.FindPropertyRelative("m_Offset");
            
            PropertyField fieldOption = new PropertyField(option, property.displayName);
            
            PropertyElement fieldTarget = new PropertyElement(
                target.FindPropertyRelative(IPropertyDrawer.PROPERTY_NAME),
                target.displayName, true
            );

            PropertyField fieldRadius = new PropertyField(radius);
            PropertyField fieldOffset = new PropertyField(offset);
            
            head.Add(fieldOption);
            
            fieldOption.RegisterValueChangeCallback(changeEvent =>
            {
                body.Clear();
                if (changeEvent.changedProperty.enumValueIndex != 1) return;
                
                body.Add(fieldTarget);
                body.Add(fieldRadius);
                body.Add(fieldOffset);
                
                body.Bind(changeEvent.changedProperty.serializedObject);
            });

            if (option.enumValueIndex == 1)
            {
                body.Add(fieldTarget);
                body.Add(fieldRadius);
                body.Add(fieldOffset);
            }

            root.Add(head);
            root.Add(body);
            
            return root;
        }
    }
}
