using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TypeSelectorListFancy : TTypeSelectorList<Button>
    {
        protected TypeSelectorListFancy(SerializedProperty propertyList, Type type, Button element) 
            : base(propertyList, type, element)
        { }
        
        protected override void SetupActivator(Button element)
        {
            element.clicked += () => TypeSelectorFancyPopup.Open(
                element, 
                this.m_Type, 
                this.OnSelectType
            );
        }
    }
}