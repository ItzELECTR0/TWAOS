using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct RotationMarker : IRotation
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Marker m_Marker;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RotationMarker(Marker marker)
        {
            this.m_Marker = marker;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasRotation(GameObject source) => this.m_Marker != null;
        
        public Quaternion GetRotation(GameObject source) => this.m_Marker.GetRotation(source);
    }
}