using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public readonly struct ShieldInput
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        /// <summary>
        /// The normalized direction in Local Space from the hit character's perspective
        /// </summary>
        [field: NonSerialized] public Vector3 Direction { get; }
        
        /// <summary>
        /// Point of impact in world space
        /// </summary>
        [field: NonSerialized] public Vector3 Point { get; }
        
        /// <summary>
        /// A value defined by the weapon that describes how strong the attack is
        /// </summary>
        [field: NonSerialized] public float Power { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShieldInput(Vector3 direction, Vector3 point, float power)
        {
            this.Direction = direction;
            this.Point = point;
            this.Power = power;
        }
    }
}