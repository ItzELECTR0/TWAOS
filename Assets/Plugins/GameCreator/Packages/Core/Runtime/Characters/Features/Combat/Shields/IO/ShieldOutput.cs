using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public readonly struct ShieldOutput
    {
        public static readonly ShieldOutput NO_BLOCK = new ShieldOutput(
            false, Vector3.zero, 
            0f, BlockType.None
        );
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        /// <summary>
        /// Whether the attack was successfully blocked
        /// </summary>
        [field: NonSerialized] public bool IsBlocked { get; }
        
        /// <summary>
        /// Point of impact in world space
        /// </summary>
        [field: NonSerialized] public Vector3 Point { get; }
        
        /// <summary>
        /// The time elapsed since the character first raised the Shield and the attack was
        /// blocked
        /// </summary>
        [field: NonSerialized] public float ElapsedTime { get; }
        
        /// <summary>
        /// Whether the attack has been perfectly blocked or not
        /// </summary>
        [field: NonSerialized] public BlockType Type { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShieldOutput(bool isBlocked, Vector3 point, float elapsedTime, BlockType type)
        {
            this.IsBlocked = isBlocked;
            this.Point = point;
            this.ElapsedTime = elapsedTime;
            this.Type = type;
        }
    }
}