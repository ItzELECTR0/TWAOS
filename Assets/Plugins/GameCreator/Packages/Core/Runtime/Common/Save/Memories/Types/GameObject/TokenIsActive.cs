using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenIsActive : Token
    {
        [SerializeField]
        private bool m_IsActive;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsActive => this.m_IsActive;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenIsActive(GameObject target) : base()
        {
            this.m_IsActive = target.activeSelf;
        }
    }
}