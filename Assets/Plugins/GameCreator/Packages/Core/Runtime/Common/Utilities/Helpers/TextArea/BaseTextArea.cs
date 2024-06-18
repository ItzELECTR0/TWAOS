using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class BaseTextArea
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private string m_Text;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public string Text => this.m_Text;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected BaseTextArea()
        {
            this.m_Text = string.Empty;
        }

        protected BaseTextArea(string text) : this()
        {
            this.m_Text = text;
        }
    }
}