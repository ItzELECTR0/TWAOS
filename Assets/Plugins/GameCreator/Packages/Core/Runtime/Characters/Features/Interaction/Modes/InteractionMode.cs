using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class InteractionMode
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeReference] private TInteractionMode m_InteractionMode;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InteractionMode()
        {
            this.m_InteractionMode = new InteractionModeNearCharacter();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float CalculatePriority(Character character, IInteractive interactive)
        {
            return this.m_InteractionMode?.CalculatePriority(character, interactive) ?? float.MaxValue;
        }

        // DRAW GIZMOS: ---------------------------------------------------------------------------

        public void DrawGizmos(Character character)
        {
            this.m_InteractionMode?.DrawGizmos(character);
        }
    }
}