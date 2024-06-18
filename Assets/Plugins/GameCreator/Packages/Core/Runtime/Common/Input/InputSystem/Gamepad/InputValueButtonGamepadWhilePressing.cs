using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad While Pressing")]
    [Category("Gamepad/Gamepad While Pressing")]
    
    [Description("While the specified gamepad or joystick button is being held down")]
    [Image(typeof(IconGamepad), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Key", "Button", "Down", "Held", "Hold")]

    [Serializable]
    public class InputValueButtonGamepadWhilePressing : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GamepadButton m_Button = GamepadButton.South;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(GamepadButton button = GamepadButton.South)
        {
            return new InputPropertyButton(
                new InputValueButtonGamepadWhilePressing
                {
                    m_Button = button
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Gamepad.all.Count <= 0) return;
            
            if (Gamepad.current[this.m_Button].wasPressedThisFrame)
            {
                this.ExecuteEventStart();   
            }
            
            if (Gamepad.current[this.m_Button].IsPressed())
            {
                this.ExecuteEventPerform();
            }
        }
    }
}