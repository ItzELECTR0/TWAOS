using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Secondary Motion")]
    [Category("Usage/Secondary Motion")]
    
    [Description("Secondary motion commonly used to orbit the camera around the main character: Move the Mouse and Right Stick on Gamepads")]
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]
    
    [Keywords("Orbit", "Joystick")]
    
    [Serializable]
    public class InputValueVector2MotionSecondary : TInputValueVector2
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
                        "Secondary Motion",
                        InputActionType.Value
                    );
                
                    this.m_InputAction.AddBinding(
                        "<Mouse>/delta",
                        processors: @"
                        invertVector2(invertX=false,invertY=true),
                        scaleVector2(x=3,y=3),
                        divideScreenSize,
                        divideDeltaTime"
                    );
                
                    this.m_InputAction.AddBinding(
                        "<Gamepad>/rightStick",
                        processors: "invertVector2(invertX=false,invertY=true)"
                    );
                }
        
                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2MotionSecondary()
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