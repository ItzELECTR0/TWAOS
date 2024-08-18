using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenComponent : Token
    {
        [SerializeField]
        private bool m_Enabled;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Enabled => this.m_Enabled;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenComponent(Behaviour behaviour) : base()
        {
            this.m_Enabled = behaviour == null || behaviour.enabled;
        }
    }
}