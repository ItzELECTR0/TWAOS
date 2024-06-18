using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenScale : Token
    {
        [SerializeField]
        private Vector3 m_Scale;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Vector3 Scale => this.m_Scale;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenScale(GameObject target) : base()
        {
            this.m_Scale = target.transform.localScale;
        }
    }
}