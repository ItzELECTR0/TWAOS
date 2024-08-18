using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenLayers : Token
    {
        [SerializeField]
        private int m_Layers;

        // PROPERTIES: ----------------------------------------------------------------------------

        public int Layers => this.m_Layers;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenLayers(GameObject target) : base()
        {
            this.m_Layers = target.layer;
        }
    }
}