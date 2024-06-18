using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action Perform (Button)")]
    [Category("Input System/Input Action Perform (Button)")]
    
    [Description("When an Input Action asset of Button type runs the Performed phase")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    
    [Keywords("Unity", "Asset", "Map", "Release")]
    
    [Serializable]
    public class InputButtonInputActionPerform : TInputButton
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
            this.m_Input.InputAction.performed -= this.OnInputPerform;
            
            this.m_Input.InputAction.canceled += this.OnInputCancel;
            this.m_Input.InputAction.started += this.OnInputStart;
            this.m_Input.InputAction.performed += this.OnInputPerform;
        }

        public override void OnDispose()
        {
            base.OnDispose();
            if (this.m_Input?.InputAction == null) return;
            
            this.m_Input.InputAction.canceled -= this.OnInputCancel;
            this.m_Input.InputAction.started -= this.OnInputStart;
            this.m_Input.InputAction.performed -= this.OnInputPerform;
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

        private void OnInputPerform(InputAction.CallbackContext _)
        {
            this.ExecuteEventPerform();
        }
    }
}