using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TypeReferenceDrawer : PropertyDrawer
    {
        protected abstract Type Base { get; }
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty typeName = property.FindPropertyRelative("m_TypeName");
            SelectTypeElement selector = new SelectTypeElement(typeName, this.Base, property.displayName); 
            
            root.Add(selector);
            return root;
        }
    }
}