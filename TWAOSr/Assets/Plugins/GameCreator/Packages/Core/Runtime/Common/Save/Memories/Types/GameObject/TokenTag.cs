using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenTag : Token
    {
        [SerializeField]
        private string m_Tag;

        // PROPERTIES: ----------------------------------------------------------------------------

        public string Tag => this.m_Tag;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenTag(GameObject target) : base()
        {
            this.m_Tag = target.tag;
        }
    }
}