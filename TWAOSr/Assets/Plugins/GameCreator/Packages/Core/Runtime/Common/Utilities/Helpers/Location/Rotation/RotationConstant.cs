using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct RotationConstant : IRotation
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Quaternion m_Rotation;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RotationConstant(Quaternion rotation)
        {
            this.m_Rotation = rotation;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasRotation(GameObject source) => true;
        
        public Quaternion GetRotation(GameObject source) => this.m_Rotation;
    }
}