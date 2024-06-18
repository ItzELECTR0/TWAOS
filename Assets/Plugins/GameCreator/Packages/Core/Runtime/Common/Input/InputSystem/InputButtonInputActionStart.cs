using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action Start (Button)")]
    [Category("Input System/Input Action Start (Button)")]
    
    [Description("When an Input Action asset of Button type runs the Started phase")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayArrowLeft))]
    
    [Keywords("Unity", "Asset", "Map", "Press")]
    
    [Serializable]
    public class InputButtonInputActionStart : TInputButton
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
            this.ExecuteEventPerform();
        }

        private void OnInputCancel(InputAction.CallbackContext _)
        {
            this.ExecuteEventCancel();
        }
    }
}