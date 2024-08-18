using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TEnablerValue<T> : TEnablerValueCommon
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private T m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public T Value
        {
            get => this.m_Value;
            set => this.m_Value = value;
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TEnablerValue() : base()
        {
            this.m_Value = default;
        }

        protected TEnablerValue(bool isEnabled, T value) : base(isEnabled)
        {
            this.m_Value = value;
        }
    }
}