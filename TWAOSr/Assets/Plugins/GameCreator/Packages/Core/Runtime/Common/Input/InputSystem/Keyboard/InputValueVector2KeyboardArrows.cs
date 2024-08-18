using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Arrow Keys")]
    [Category("Keyboard/Arrow Keys")]
    
    [Description("Detects when the arrow keys are pressed")]
    [Image(typeof(IconWASD), ColorTheme.Type.Yellow)]
    
    [Keywords("Move")]
    
    [Serializable]
    public class InputValueVector2KeyboardArrows : TInputValueVector2
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
                        "WASD",
                        InputActionType.Value
                    );

                    this.m_InputAction.AddCompositeBinding("2DVector")
                        .With("Up", "<Keyboard>/upArrow")
                        .With("Down", "<Keyboard>/downArrow")
                        .With("Left", "<Keyboard>/leftArrow")
                        .With("Right", "<Keyboard>/rightArrow");
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2KeyboardWASD()
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