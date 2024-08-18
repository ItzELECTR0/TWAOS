using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Marker Type")]
    
    [Serializable]
    public abstract class TMarkerType
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public abstract Vector3 GetPosition(Marker marker, GameObject user);
        public abstract Vector3 GetDirection(Marker marker, GameObject user);
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public abstract void OnDrawGizmos(Marker marker);
    }
}