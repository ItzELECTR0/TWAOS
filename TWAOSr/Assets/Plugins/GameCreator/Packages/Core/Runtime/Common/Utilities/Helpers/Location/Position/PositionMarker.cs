using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct PositionMarker : IPosition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Marker m_Marker;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PositionMarker(Marker marker)
        {
            this.m_Marker = marker;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasPosition(GameObject user) => user != null && this.m_Marker != null;
        
        public Vector3 GetPosition(GameObject source) => this.m_Marker.GetPosition(source);
    }
}