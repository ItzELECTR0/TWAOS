using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Delta")]
    [Category("Mouse/Mouse Delta")]
    
    [Description("The delta position from the last cursor position")]
    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    
    [Keywords("Cursor", "Move", "Pan")]
    
    [Serializable]
    public class InputValueVector2MouseDelta : TInputValueVector2
    {
        private enum MouseButton
        {
            Always,
            PressingLeftButton,
            PressingMiddleButton,
            PressingRightButton
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;

        [SerializeField] private MouseButton m_WhilePressing = MouseButton.Always;

        // PROPERTIES: ----------------------------------------------------------------------------

        public InputAction InputAction
        {
            get
            {
                if (this.m_InputAction == null)
                {
                    this.m_InputAction = new InputAction(
                        name: "Mouse Delta", 
                        type: InputActionType.Value,
                        binding: "<Mouse>/delta"
                    );
                }
                
                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2MouseDelta()
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
            Vector2 movement = this.InputAction?.ReadValue<Vector2>() ?? Vector2.zero;

            return this.m_WhilePressing switch
            {
                MouseButton.PressingLeftButton => Mouse.current.leftButton.isPressed 
                    ? movement 
                    : Vector2.zero,
                
                MouseButton.PressingMiddleButton => Mouse.current.middleButton.isPressed 
                    ? movement 
                    : Vector2.zero,
                
                MouseButton.PressingRightButton => Mouse.current.rightButton.isPressed
                    ? movement 
                    : Vector2.zero,
                
                _ => movement
            };
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