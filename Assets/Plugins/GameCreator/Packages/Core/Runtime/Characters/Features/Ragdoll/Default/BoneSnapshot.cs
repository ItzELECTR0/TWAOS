using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    internal class BoneSnapshot
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public Transform Value { get; }

        [field: NonSerialized] public Vector3 LocalPosition { get; }
        [field: NonSerialized] public Vector3 WorldPosition { get; }

        [field: NonSerialized] public Quaternion LocalRotation { get; }
        [field: NonSerialized] public Quaternion WorldRotation { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public BoneSnapshot(Transform reference)
        {
            this.Value = reference;
            
            this.WorldPosition = this.Value.position;
            this.LocalPosition = this.Value.localPosition;

            this.WorldRotation = this.Value.rotation;
            this.LocalRotation = this.Value.localRotation;
        }
    }
}