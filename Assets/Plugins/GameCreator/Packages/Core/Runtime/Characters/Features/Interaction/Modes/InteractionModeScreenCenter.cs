using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Screen Center")]
    [Category("Screen Center")]
    
    [Image(typeof(IconCamera), ColorTheme.Type.Green)]
    [Description("Selects the interactive element that's closest to the center of the screen")]
    
    [Serializable]
    public class InteractionModeScreenCenter : TInteractionMode
    {
        [SerializeField] private float m_MaxDistance = 0.5f;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override float CalculatePriority(Character character, IInteractive interactive)
        {
            Transform camera = ShortcutMainCamera.Transform;
            if (camera == null) return float.MaxValue;

            Vector3 cameraDirection = camera.TransformDirection(Vector3.forward);
            Vector3 interactiveDirection = interactive.Position - camera.position;

            if (Vector3.Dot(cameraDirection, interactiveDirection) < 0f) return float.MaxValue; 
            
            float distance = Vector3.Cross(
                cameraDirection, 
                interactiveDirection
            ).magnitude;

            return distance < this.m_MaxDistance ? distance : float.MaxValue;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public override void DrawGizmos(Character character)
        {
            base.DrawGizmos(character);

            Vector3 normal = character.transform.TransformDirection(Vector3.forward);
            Vector3 position = character.Eyes + normal * 0.5f;

            Gizmos.color = COLOR_GIZMOS;
            GizmosExtension.Circle(position, this.m_MaxDistance, normal);
        }
    }
}