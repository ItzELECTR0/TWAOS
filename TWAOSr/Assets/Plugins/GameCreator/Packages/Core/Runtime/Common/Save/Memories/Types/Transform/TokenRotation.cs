using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenRotation : Token
    {
        [SerializeField]
        private Quaternion m_Rotation;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Quaternion Rotation => this.m_Rotation;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenRotation(GameObject target) : base()
        {
            this.m_Rotation = target.transform.rotation;
        }
    }
}