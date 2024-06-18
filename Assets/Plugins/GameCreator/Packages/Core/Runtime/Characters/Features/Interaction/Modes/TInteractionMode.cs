using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Interaction Mode")]
    
    [Serializable]
    public abstract class TInteractionMode
    {
        protected static readonly Color COLOR_GIZMOS = new Color(0f, 1f, 0f, 0.5f);
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public abstract float CalculatePriority(Character character, IInteractive interactive);

        // DRAW GIZMOS: ---------------------------------------------------------------------------

        public virtual void DrawGizmos(Character character)
        { }
    }
}