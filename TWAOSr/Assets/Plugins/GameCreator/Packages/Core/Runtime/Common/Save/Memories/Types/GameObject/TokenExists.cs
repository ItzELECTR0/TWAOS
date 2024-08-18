using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenExists : Token
    {
        [SerializeField]
        private bool m_Exists;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Exists => this.m_Exists;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenExists(GameObject target) : base()
        {
            if (target == null)
            {
                this.m_Exists = false;
                return;
            }

            Remember remember = target.Get<Remember>();
            if (remember == null)
            {
                this.m_Exists = false;
                return;
            }

            this.m_Exists = !remember.IsSceneLoaded || !remember.IsDestroying;
        }
    }
}