using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class InputPropertyValueVector2 : TInputProperty
    {
        [SerializeReference] private TInputValueVector2 m_Input;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override TInput Input => this.m_Input;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InputPropertyValueVector2()
        {
            this.m_Input = new InputValueVector2None();
        }
        
        public InputPropertyValueVector2(TInputValueVector2 input)
        {
            this.m_Input = input;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector2 Read() => this.m_Input.Read();
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => this.Input.ToString();
    }
}