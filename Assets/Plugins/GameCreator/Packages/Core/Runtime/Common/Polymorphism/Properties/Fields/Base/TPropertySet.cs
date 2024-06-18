using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TPropertySet<TType, TValue> : IProperty where TType : TPropertyTypeSet<TValue>
    {
        [SerializeReference]
        protected TType m_Property;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TPropertySet(TType defaultType)
        {
            this.m_Property = defaultType;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Set(TValue value, Args args) => this.m_Property.Set(value, args);
        public virtual void Set(TValue value, GameObject target) => this.m_Property.Set(value, target);

        public virtual void Set(TValue value, Component component)
        {
            this.Set(value, component ? component.gameObject : null);
        }
        
        public virtual TValue Get(Args args) => this.m_Property.Get(args);
        public virtual TValue Get(GameObject target) => this.m_Property.Get(target);

        public virtual TValue Set(Component component)
        {
            return this.Get(component ? component.gameObject : null);
        }

        public override string ToString()
        {
            return this.m_Property?.String ?? "(none)";
        }
    }
}