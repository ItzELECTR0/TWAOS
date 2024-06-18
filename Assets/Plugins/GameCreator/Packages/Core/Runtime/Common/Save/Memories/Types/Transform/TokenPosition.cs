using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TokenPosition : Token
    {
        [SerializeField]
        private Vector3 m_Position;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Vector3 Position => this.m_Position;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public TokenPosition(GameObject target) : base()
        {
            Character character = target.Get<Character>();
            this.m_Position = character != null 
                ? character.Feet 
                : target.transform.position;
        }
    }
}