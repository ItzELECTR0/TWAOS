using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Characters
{
    [Title("Screen Cursor")]
    [Category("Screen Cursor")]
    
    [Image(typeof(IconCursor), ColorTheme.Type.Green)]
    [Description("Selects the interactive element that's closest to the cursor on the screen")]
    
    [Serializable]
    public class InteractionModeScreenCursor : TInteractionMode
    {
        [SerializeField] private float m_MaxDistance = 0.5f;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override float CalculatePriority(Character character, IInteractive interactive)
        {
            Camera camera = ShortcutMainCamera.Get<Camera>();
            if (camera == null) return float.MaxValue;

            Vector3 direction = Cursor.lockState == CursorLockMode.Locked
                ? camera.transform.TransformDirection(Vector3.forward)
                : camera.ScreenPointToRay(Mouse.current.position.ReadValue()).direction;
            
            Vector3 interactiveDirection = interactive.Position - camera.transform.position;
            if (Vector3.Dot(direction, interactiveDirection) < 0f) return float.MaxValue;
            
            float distance = Vector3.Cross(
                direction, 
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