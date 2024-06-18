using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenLightIntensity : Token
    {
        [SerializeField]
        private float m_Intensity;

        // PROPERTIES: ----------------------------------------------------------------------------

        public float Intensity => this.m_Intensity;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenLightIntensity(Light light) : base()
        {
            this.m_Intensity = light != null ? light.intensity : 0f;
        }
    }
}