using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class ListTypeElement : TTypeSelector<Button>
    {
        private readonly Action<Type> m_OnChange;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ListTypeElement(Button element, Action<Type> onChange)
            : base(typeof(TValue), element)
        {
            this.m_OnChange = onChange;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected override void SetupActivator(Button element)
        {
            element.clicked += () => TypeSelectorFancyPopup.Open(
                element, 
                this.m_Type, 
                this.OnSelectType
            );
        }

        private void OnSelectType(Type newType)
        {
            this.m_OnChange?.Invoke(newType);
            this.InvokeEventChange(null, newType);
        }
    }
}