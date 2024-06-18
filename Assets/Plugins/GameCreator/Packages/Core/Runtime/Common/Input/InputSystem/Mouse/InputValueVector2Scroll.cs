using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Scroll")]
    [Category("Mouse/Mouse Scroll")]
    
    [Description("Every time the scroll wheel is used")]
    [Image(typeof(IconScroll), ColorTheme.Type.Yellow)]
    
    [Keywords("Cursor", "Button", "Up")]
    
    [Serializable]
    public class InputValueVector2Scroll : TInputValueVector2
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
                        name: "Zoom", 
                        type: InputActionType.Value,
                        binding: "<Mouse>/scroll"
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2Scroll()
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