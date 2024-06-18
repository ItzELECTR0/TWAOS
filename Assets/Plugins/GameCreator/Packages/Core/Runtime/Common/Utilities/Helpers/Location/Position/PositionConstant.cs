using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct PositionConstant : IPosition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Vector3 m_Position;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PositionConstant(Vector3 position)
        {
            this.m_Position = position;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasPosition(GameObject user) => true;
        
        public Vector3 GetPosition(GameObject source) => this.m_Position;
    }
}