using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Interact")]
    [Category("Usage/Interact")]
    
    [Description("Cross-device support for the 'Interact' skill: E key on Keyboards and pressing the West Stick on Gamepads")]
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green)]

    [Serializable]
    public class InputButtonInteract : TInputButtonInputAction
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
                        "Interact",
                        InputActionType.Button
                    );

                    this.m_InputAction.AddBinding("<Gamepad>/buttonWest");
                    this.m_InputAction.AddBinding("<Keyboard>/e");
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
                new InputButtonInteract()
            );
        }
    }
}