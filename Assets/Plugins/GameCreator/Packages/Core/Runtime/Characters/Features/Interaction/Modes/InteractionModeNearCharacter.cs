using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Near Character")]
    [Category("Near Character")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Green)]
    [Description("Selects the closest interactive element to the Character")]
    
    [Serializable]
    public class InteractionModeNearCharacter : TInteractionMode
    {
        private static readonly Vector3 GIZMO_SIZE = Vector3.one * 0.05f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Vector3 m_Offset = new Vector3(0f, 0f, 1f);

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override float CalculatePriority(Character character, IInteractive interactive)
        {
            if (character == null) return float.MaxValue;
            
            return Vector3.Distance(
                character.transform.TransformPoint(this.m_Offset), 
                interactive.Position
            );
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public override void DrawGizmos(Character character)
        {
            base.DrawGizmos(character);

            Vector3 position = character.transform.TransformPoint(this.m_Offset);

            Gizmos.color = COLOR_GIZMOS;
            Gizmos.DrawCube(position, GIZMO_SIZE);
        }
    }
}