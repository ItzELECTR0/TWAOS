using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Crouch")]
    [Category("Usage/Crouch")]
    
    [Description("Cross-device support for the 'Crouch' skill: Ctrl key on Keyboards and pressing the Right Stick on Gamepads")]
    [Image(typeof(IconCharacterCrouch), ColorTheme.Type.Green)]

    [Serializable]
    public class InputButtonCrouch : TInputButtonInputAction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override InputAction InputAction
        {
            get
            {
                if (this.m_InputAction == null)
                {
                    this.m_InputAction = new InputAction(
                        "Crouch",
                        InputActionType.Button
                    );

                    this.m_InputAction.AddBinding("<Gamepad>/rightStickPress");
                    this.m_InputAction.AddBinding("<Keyboard>/ctrl");
                }

                return this.m_InputAction;
            }
        }
        
        // PROTECTED OVERRIDE METHODS: ------------------------------------------------------------
        
        protected override void ExecuteEventStart(InputAction.CallbackContext context)
        {
            this.ExecuteEventStart();
        }
        
        protected override void ExecuteEventCancel(InputAction.CallbackContext context)
        {
            this.ExecuteEventCancel();
        }
        
        protected override void ExecuteEventPerform(InputAction.CallbackContext context)
        {
            this.ExecuteEventPerform();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonCrouch()
            );
        }
    }
}