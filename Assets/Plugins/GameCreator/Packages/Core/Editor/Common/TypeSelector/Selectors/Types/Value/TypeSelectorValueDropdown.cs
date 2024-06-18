using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorValueDropdown : TTypeSelectorValue<VisualElement>
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public PopupField<Type> PopupField { get; private set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TypeSelectorValueDropdown(SerializedProperty property, VisualElement element,
            string label) : base(property, element)
        {
            this.PopupField.label = label;
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------

        protected override void SetupActivator(VisualElement element)
        {
            Type typeFull = TypeUtils.GetTypeFromProperty(this.m_Property, true);
            Type typeField = TypeUtils.GetTypeFromProperty(this.m_Property, false);
            
            List<Type> typesCollection = GetCandidateTypes(typeField);
            
            this.PopupField = new PopupField<Type>(
                typesCollection, typeFull,
                type => TypeUtils.GetTitleFromType(type),
                type => TypeUtils.GetTitleFromType(type)
            );

            this.PopupField.RegisterValueChangedCallback(changeEvent =>
            {
                typeFull = changeEvent.newValue;
                this.OnSelectType(changeEvent.newValue);
            });
            
            element.Add(this.PopupField);
        }
    }
}