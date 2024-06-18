using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TTypeSelectorList<T> : TTypeSelector<T> where T : VisualElement 
    {
        // MEMBERS: -------------------------------------------------------------------------------

        protected readonly SerializedProperty m_PropertyList;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TTypeSelectorList(SerializedProperty propertyList, Type type, T element) 
            : base(type, element)
        {
            this.m_PropertyList = propertyList;
        }
        
        protected void OnSelectType(Type newType)
        {
            this.InvokeEventChange(null, newType);
        }
    }
}
