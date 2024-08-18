using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class InputPropertyValueFloat : TInputProperty
    {
        [SerializeReference] private TInputValueFloat m_Input;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override TInput Input => this.m_Input;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InputPropertyValueFloat()
        {
            this.m_Input = new InputValueFloatNone();
        }
        
        public InputPropertyValueFloat(TInputValueFloat input)
        {
            this.m_Input = input;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Read() => this.m_Input.Read();
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => this.Input.ToString();
    }
}