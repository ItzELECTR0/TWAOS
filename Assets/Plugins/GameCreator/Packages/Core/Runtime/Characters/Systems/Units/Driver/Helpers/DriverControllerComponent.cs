using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [AddComponentMenu("")]
    public class DriverControllerComponent : MonoBehaviour
    {
        private event Action<ControllerColliderHit> EventControllerColliderHit;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.hideFlags = HideFlags.HideInInspector |
                             HideFlags.HideInHierarchy |
                             HideFlags.DontSave;
        }

        private void OnDestroy()
        {
            this.EventControllerColliderHit = null;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static DriverControllerComponent Register(
            Character self, Action<ControllerColliderHit> onHitCallback)
        {
            DriverControllerComponent component = self.gameObject
                .AddComponent<DriverControllerComponent>();
            
            component.EventControllerColliderHit += onHitCallback;

            return component;
        }

        // CALLBACKS: -----------------------------------------------------------------------------

        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            this.EventControllerColliderHit?.Invoke(hit);
        }
    }
}