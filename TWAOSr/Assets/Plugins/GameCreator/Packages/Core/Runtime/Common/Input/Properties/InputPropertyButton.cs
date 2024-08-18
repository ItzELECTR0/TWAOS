using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class InputPropertyButton : TInputProperty
    {
        [SerializeReference] private TInputButton m_Input;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override TInput Input => this.m_Input;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InputPropertyButton()
        {
            this.m_Input = new InputButtonNone();
        }
        
        public InputPropertyButton(TInputButton input)
        {
            this.m_Input = input;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void RegisterStart(Action callback)
        {
            this.m_Input.EventStart -= callback;
            this.m_Input.EventStart += callback;
        }
        
        public void RegisterCancel(Action callback)
        {
            this.m_Input.EventCancel -= callback;
            this.m_Input.EventCancel += callback;
        }
        
        public void RegisterPerform(Action callback)
        {
            this.m_Input.EventPerform -= callback;
            this.m_Input.EventPerform += callback;
        }
        
        public void ForgetStart(Action callback) => this.m_Input.EventStart -= callback;
        public void ForgetCancel(Action callback) => this.m_Input.EventCancel -= callback;
        public void ForgetPerform(Action callback) => this.m_Input.EventPerform -= callback;
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => this.Input.ToString();
    }
}