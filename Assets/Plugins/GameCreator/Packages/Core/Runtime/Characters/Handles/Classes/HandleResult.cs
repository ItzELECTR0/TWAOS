using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public struct HandleResult
    {
        public static readonly HandleResult None = new HandleResult(
            new Bone(),
            Vector3.zero,
            Quaternion.identity
        );
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public Bone Bone { get; }
        
        [field: NonSerialized] public Vector3 LocalPosition { get; }
        [field: NonSerialized] public Quaternion LocalRotation { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public HandleResult(Bone bone, Vector3 position, Quaternion rotation)
        {
            this.Bone = bone;
            this.LocalPosition = position;
            this.LocalRotation = rotation;
        }
    }
}