using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TagValue))]
    public class TagValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("m_Value");
            TagField tagField = new TagField(property.displayName, value.stringValue);
            
            tagField.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);

            tagField.RegisterValueChangedCallback(changeEvent =>
            {
                value.serializedObject.Update();
                value.stringValue = changeEvent.newValue;
                
                SerializationUtils.ApplyUnregisteredSerialization(value.serializedObject);
            });
            
            return tagField;
        }
    }
}