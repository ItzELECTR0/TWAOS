using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Jump")]
    [Category("Usage/Jump")]
    
    [Description("Cross-device support for the 'Jump' skill: Space key on Keyboards and the South Button on Gamepads")]
    [Image(typeof(IconCharacterJump), ColorTheme.Type.Green)]

    [Serializable]
    public class InputButtonJump : TInputButtonInputAction
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
                        "Jump",
                        InputActionType.Button
                    );

                    this.m_InputAction.AddBinding("<Gamepad>/buttonSouth");
                    this.m_InputAction.AddBinding("<Keyboard>/space");
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
                new InputButtonJump()
            );
        }
    }
}