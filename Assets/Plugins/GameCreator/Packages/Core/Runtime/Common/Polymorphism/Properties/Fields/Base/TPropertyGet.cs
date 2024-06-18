using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TPropertyGet<TType, TValue> : IProperty
        where TType : TPropertyTypeGet<TValue>
    {
        [SerializeReference]
        protected TType m_Property;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TPropertyGet(TType defaultType)
        {
            this.m_Property = defaultType;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual TValue Get(Args args) => this.m_Property != null 
            ? this.m_Property.Get(args) 
            : default;

        public virtual TValue Get(GameObject target) => this.m_Property != null
            ? this.m_Property.Get(target)
            : default;

        public virtual TValue Get(Component component)
        {
            return this.Get(component != null ? component.gameObject : null);
        }

        public override string ToString()
        {
            return this.m_Property?.String ?? "(none)";
        }
        
        // EDITOR: --------------------------------------------------------------------------------

        /// <summary>
        /// EDITOR ONLY: This is used by editor scripts that require an optional scene value for
        /// tooling, like displaying the radius of a field. Only use if the value is not dynamic.
        /// </summary>
        public TValue EditorValue => this.m_Property.EditorValue;
    }
}