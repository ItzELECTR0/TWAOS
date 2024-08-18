using System;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TTypeSelectorElement : VisualElement
    {
        // MEMBERS: -------------------------------------------------------------------------------

        protected readonly VisualElement m_Root;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string ElementNameRoot { get; }
        protected abstract string ElementNameHead { get; }
        protected abstract string ElementNameBody { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TTypeSelectorElement()
        {
            this.m_Root = new VisualElement { name = this.ElementNameRoot };
            
            this.Add(this.m_Root);
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void OnChange(Type prevType, Type newType);
    }   
}
