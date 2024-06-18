using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Walk")]
    [Category("Usage/Walk")]
    
    [Description("Cross-device support for the 'Walk' skill: Z key on Keyboards and the pressing Left Stick on Gamepads")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Green)]

    [Serializable]
    public class InputButtonWalk : TInputButtonInputAction
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
                        "Walk",
                        InputActionType.Button
                    );

                    this.m_InputAction.AddBinding("<Gamepad>/leftStickPress");
                    this.m_InputAction.AddBinding("<Keyboard>/z");
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
                new InputButtonWalk()
            );
        }
    }
}