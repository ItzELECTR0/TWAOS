using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad Right Stick")]
    [Category("Gamepad/Gamepad Right Stick")]
    
    [Description("The Right Stick direction")]
    [Image(typeof(IconJoystick), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Keywords("Cursor", "Location", "Move", "Pan")]
    
    [Serializable]
    public class InputValueFloatGamepadRightStick : TInputValueFloat
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
                        name: "Right Stick", 
                        type: InputActionType.Value,
                        binding: "<Gamepad>/rightStick"
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2GamepadRightStick()
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

        public override float Read()
        {
            return this.InputAction?.ReadValue<Vector2>().magnitude ?? 0f;
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