using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad Left Stick")]
    [Category("Gamepad/Gamepad Left Stick")]
    
    [Description("The Left Stick direction")]
    [Image(typeof(IconJoystick), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    
    [Keywords("Cursor", "Location", "Move", "Pan")]
    
    [Serializable]
    public class InputValueVector2GamepadLeftStick : TInputValueVector2
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public InputAction InputAction
        {
            get
            {
                if (this.m_InputAction == null)
                {
                    this.m_InputAction = new InputAction(
                        name: "Left Stick", 
                        type: InputActionType.Value,
                        binding: "<Gamepad>/leftStick"
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2GamepadLeftStick()
            );
        }

        public override void OnStartup()
        {
            this.Enable();
        }

        public override void OnDispose()
        {
            this.Disable();
            this.InputAction?.Dispose();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Vector2 Read()
        {
            return this.InputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Enable()
        {
            this.InputAction?.Enable();
        }

        private void Disable()
        {
            this.InputAction?.Disable();
        }
    }
}