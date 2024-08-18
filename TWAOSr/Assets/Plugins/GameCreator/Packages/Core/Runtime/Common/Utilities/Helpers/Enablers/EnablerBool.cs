using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerBool : TEnablerValueCommon
    {
        public enum Bool
        {
            Off = 0,
            On = 1
        }

        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Bool m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Value
        {
            get => this.m_Value switch
            {
                Bool.Off => false,
                Bool.On => true,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            set => this.m_Value = value switch
            {
                true => Bool.On,
                false => Bool.Off
            };
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public EnablerBool() : this(false, true)
        { }

        public EnablerBool(bool value) : this(false, value)
        { }

        public EnablerBool(bool isEnabled, bool value) : base(isEnabled)
        {
            this.Value = value;
        }
    }
}