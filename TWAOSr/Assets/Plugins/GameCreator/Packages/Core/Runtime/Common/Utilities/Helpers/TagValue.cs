using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TagValue
    {
        [SerializeField] private string m_Value = "Untagged";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Value => this.m_Value;
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString() => this.m_Value;
    }
}