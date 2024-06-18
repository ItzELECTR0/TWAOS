using System;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Cursor Click")]
    [Category("Input/On Cursor Click")]
    [Description("Detects when the cursor clicks this game object")]

    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    [Keywords("Down", "Mouse", "Button", "Hover")]

    [Serializable]
    public class EventOnCursorClick : TEventMouse
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private int m_PressCount = 1;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private RaycastHit m_Hit3D;
        private RaycastHit2D m_Hit2D;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override bool RequiresCollider => true;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override bool InteractionSuccessful(Trigger trigger)
        {
            bool pressCount = this.m_PressCount == this.PressCount;
            return this.WasPressedThisFrame && pressCount && this.CheckRaycast(trigger);
        }

        private bool CheckRaycast(Trigger trigger)
        {
            if (ShortcutMainCamera.Instance == null) return false;
            
            Camera camera = ShortcutMainCamera.Instance.Get<Camera>();
            if (camera == null) return false;
            
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            bool hit = Physics.Raycast(
                ray, out this.m_Hit3D, 
                Mathf.Infinity, this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );

            if (this.RaycastHit3D(hit, trigger)) return true;

            this.m_Hit2D = Physics2D.Raycast(
                ray.origin, ray.direction, 
                Mathf.Infinity, this.m_LayerMask
            );
            
            return this.RaycastHit2D(this.m_Hit2D.collider != null, trigger);
        }

        private bool RaycastHit3D(bool hit, Trigger trigger)
        {
            if (!hit) return false;
            return this.m_Hit3D.collider.gameObject == trigger.gameObject;
        }
        
        private bool RaycastHit2D(bool hit, Trigger trigger)
        {
            if (!hit) return false;
            return this.m_Hit2D.collider.gameObject == trigger.gameObject;
        }
    }
}