using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenLightColor : Token
    {
        [SerializeField]
        private Color m_Color;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Color Color => this.m_Color;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenLightColor(Light light) : base()
        {
            this.m_Color = light != null ? light.color : Color.white;
        }
    }
}