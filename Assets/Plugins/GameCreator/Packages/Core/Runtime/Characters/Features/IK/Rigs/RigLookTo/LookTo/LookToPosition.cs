using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    public readonly struct LookToPosition : ILookTo
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public int Layer { get; }

        public bool Exists => true;
        
        [field: NonSerialized] public Vector3 Position { get; }
        
        public GameObject Target => null;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public LookToPosition(int layer, Vector3 position)
        {
            this.Layer = layer;
            this.Position = position;
        }
    }
}