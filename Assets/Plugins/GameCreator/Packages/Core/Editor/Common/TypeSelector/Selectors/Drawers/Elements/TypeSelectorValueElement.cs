using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TypeSelectorValueElement : TTypeSelectorElement
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        protected readonly VisualElement m_Head;
        protected readonly VisualElement m_Body;

        protected readonly SerializedProperty m_Property;
        
        private readonly bool m_HideLabels;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected ITypeSelector TypeSelector { get; set; }

        protected bool HideLabels => this.m_HideLabels || this.m_Property.HideLabelsInEditor();

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TypeSelectorValueElement(SerializedProperty property, bool hideLabels) : base()
        {
            this.m_Property = property;
            this.m_HideLabels = hideLabels;

            this.m_Head = new VisualElement { name = this.ElementNameHead };
            this.m_Body = new VisualElement { name = this.ElementNameBody };
            
            this.m_Root?.Add(this.m_Head);
            this.m_Root?.Add(this.m_Body);
            
            this.m_Root?.Bind(this.m_Property?.serializedObject);
            
            this.CreateHead();
            this.CreateBody();
        }
        
        // IMPLEMENT METHODS: ---------------------------------------------------------------------

        protected virtual void CreateHead()
        { }

        protected virtual void CreateBody()
        { }
        
        // CALLBACKS: -----------------------------------------------------------------------------
        
        protected override void OnChange(Type prevType, Type newType)
        {
            this.m_Body.name = this.ElementNameBody;
            this.m_Body.Clear();
            
            this.CreateBody();
            
            using ChangeEvent<Type> changeTypeEvent = ChangeEvent<Type>.GetPooled(
                prevType, newType
            );

            changeTypeEvent.target = this;
            this.SendEvent(changeTypeEvent);
        }
    }
}