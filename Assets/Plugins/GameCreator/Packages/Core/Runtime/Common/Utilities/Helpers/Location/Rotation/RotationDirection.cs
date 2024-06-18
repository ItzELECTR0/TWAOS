using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct RotationDirection : IRotation
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Vector3 m_Position;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RotationDirection(Vector3 position)
        {
            this.m_Position = position;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasRotation(GameObject source) => source != null;
        
        public Quaternion GetRotation(GameObject source)
        {
            Vector3 direction = this.m_Position - source.transform.position;
            return Quaternion.LookRotation(direction, source.transform.up);
        }
    }
}