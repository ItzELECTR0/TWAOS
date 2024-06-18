using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(IdString))]
    public class IdStringDrawer : PropertyDrawer
    {
        public const string NAME_STRING = "m_String";
        public const string NAME_HASH = "m_Hash";
        
        // PAINT METHODS: -------------------------------------------------------------------------

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative(NAME_STRING);
            PropertyField field = new PropertyField(value, property.displayName);
            
            field.RegisterValueChangeCallback(changeEvent =>
            {
                string text = changeEvent.changedProperty.stringValue;
                value.stringValue = TextUtils.ProcessID(text, false);
                
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            });

            return field;
        }
    }
}