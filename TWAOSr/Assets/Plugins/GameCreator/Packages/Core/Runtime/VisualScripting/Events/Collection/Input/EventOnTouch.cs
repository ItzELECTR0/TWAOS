using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Touch")]
    [Category("Input/On Touch")]
    [Description("Detects when a finger touches this game object on a touchscreen")]

    [Image(typeof(IconTouch), ColorTheme.Type.Yellow)]
    [Keywords("Down", "Finger", "Press", "Click")]

    [Serializable]
    public class EventOnTouch : TEventTouch
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private EnablerInt m_TapCount = new EnablerInt(false, 2);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private RaycastHit m_Hit3D;
        private RaycastHit2D m_Hit2D;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override bool RequiresCollider => true;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override bool InteractionSuccessful(Trigger trigger)
        {
            bool tapCount = !this.m_TapCount.IsEnabled || this.m_TapCount.Value == this.TapCount;
            return this.WasTouchedThisFrame && tapCount && this.CheckRaycast(trigger);
        }

        private bool CheckRaycast(Trigger trigger)
        {
            if (ShortcutMainCamera.Instance == null) return false;
            
            Camera camera = ShortcutMainCamera.Instance.Get<Camera>();
            if (camera == null) return false;

            Vector2 position = this.Position;

            Ray ray = camera.ScreenPointToRay(position);
            
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