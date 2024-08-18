using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action While Holding (Button)")]
    [Category("Input System/Input Action While Holding (Button)")]
    
    [Description("While an Input Action asset of Button type is being held down")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Unity", "Asset", "Map", "Pressing")]
    
    [Serializable]
    public class InputButtonInputActionHolding : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override void OnStartup()
        {
            base.OnStartup();
            if (this.m_Input?.InputAction == null) return;
            
            this.m_Input.InputAction.canceled -= this.OnInputCancel;
            this.m_Input.InputAction.started -= this.OnInputStart;
            
            this.m_Input.InputAction.canceled += this.OnInputCancel;
            this.m_Input.InputAction.started += this.OnInputStart;
        }

        public override void OnDispose()
        {
            base.OnDispose();
            if (this.m_Input?.InputAction == null) return;
            
            this.m_Input.InputAction.canceled -= this.OnInputCancel;
            this.m_Input.InputAction.started -= this.OnInputStart;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnInputStart(InputAction.CallbackContext _)
        {
            this.ExecuteEventStart();
        }

        private void OnInputCancel(InputAction.CallbackContext _)
        {
            this.ExecuteEventCancel();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (this.m_Input.InputAction == null) return;
            
            bool isPressed = 
                this.m_Input.InputAction.IsPressed() ||
                this.m_Input.InputAction.WasReleasedThisFrame();

            if (isPressed)
            {
                this.ExecuteEventPerform();   
            }
        }
    }
}