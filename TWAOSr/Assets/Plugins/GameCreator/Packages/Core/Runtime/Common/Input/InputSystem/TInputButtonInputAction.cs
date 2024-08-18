using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TInputButtonInputAction : TInputButton
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public abstract InputAction InputAction { get; }

        // INITIALIZERS: --------------------------------------------------------------------------
        
        public override void OnStartup()
        {
            this.Enable();
        }

        public override void OnDispose()
        {
            this.Disable();
            this.InputAction?.Dispose();
        }

        public override void OnUpdate()
        {
            this.RequireActiveInputAsset();
            base.OnUpdate();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Enable()
        {
            if (this.InputAction == null) return;
            this.RequireActiveInputAsset();
            
            this.InputAction.started -= this.ExecuteEventStart;
            this.InputAction.canceled -= this.ExecuteEventCancel;
            this.InputAction.performed -= this.ExecuteEventPerform;
            
            this.InputAction.started += this.ExecuteEventStart;
            this.InputAction.canceled += this.ExecuteEventCancel;
            this.InputAction.performed += this.ExecuteEventPerform;
        }

        private void Disable()
        {
            if (this.InputAction == null) return;
            
            this.InputAction.started -= this.ExecuteEventStart;
            this.InputAction.canceled -= this.ExecuteEventCancel;
            this.InputAction.performed -= this.ExecuteEventPerform;
        }
        
        private void RequireActiveInputAsset()
        {
            if (this.InputAction?.enabled ?? false) return;
            this.InputAction?.Enable();
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected abstract void ExecuteEventStart(InputAction.CallbackContext context);

        protected abstract void ExecuteEventCancel(InputAction.CallbackContext context);

        protected abstract void ExecuteEventPerform(InputAction.CallbackContext context);
    }
}