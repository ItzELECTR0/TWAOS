using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TTypeSelectorValue<T> : TTypeSelector<T> where T : VisualElement 
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        protected readonly SerializedProperty m_Property;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TTypeSelectorValue(SerializedProperty property, T element) 
            : base(TypeUtils.GetTypeFromProperty(property, false), element)
        {
            this.m_Property = property;
        }

        protected virtual void OnSelectType(Type newType)
        {
            this.m_Property.serializedObject.Update();
            Type prevType = TypeUtils.GetTypeFromProperty(this.m_Property, true);
            if (prevType == newType) return;

            object instance = Activator.CreateInstance(newType);
            this.m_Property.SetValue(instance);

            SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
            this.InvokeEventChange(prevType, newType);
        }
    }
}
