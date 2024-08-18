using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorFancyType : TTypeSelector<Button>
    {
        private readonly SerializedProperty m_Property;

        public TypeSelectorFancyType(SerializedProperty property, Type type, Button element)
            : base(type, element)
        {
            this.m_Property = property;
        }
        
        protected override void SetupActivator(Button element)
        {
            element.clicked += () => TypeSelectorFancyPopup.Open(
                element, 
                this.m_Type, 
                this.OnSelectType
            );
        }

        private void OnSelectType(Type type)
        {
            this.m_Property.serializedObject.Update();
            
            this.m_Property.stringValue = type.AssemblyQualifiedName;
            SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
            
            this.InvokeEventChange(null, type);
        }
    }
}
