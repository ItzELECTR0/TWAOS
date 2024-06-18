using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenName : Token
    {
        [SerializeField]
        private string m_Name;

        // PROPERTIES: ----------------------------------------------------------------------------

        public string Name => this.m_Name;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenName(GameObject target) : base()
        {
            this.m_Name = target.name;
        }
    }
}