using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TEnablerValueCommon
    {
        [SerializeField] private bool m_IsEnabled;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsEnabled
        {
            get => this.m_IsEnabled;
            set => this.m_IsEnabled = value;
        }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TEnablerValueCommon()
        {
            this.m_IsEnabled = false;
        }

        protected TEnablerValueCommon(bool isEnabled)
        {
            this.m_IsEnabled = isEnabled;
        }
    }
}