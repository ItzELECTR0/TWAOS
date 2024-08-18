using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad Release")]
    [Category("Gamepad/Gamepad Release")]
    
    [Description("When a gamepad or joystick key is released")]
    [Image(typeof(IconGamepad), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    
    [Keywords("Key", "Button", "Up", "Joystick")]
    
    [Serializable]
    public class InputValueButtonGamepadRelease : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GamepadButton m_Button = GamepadButton.South;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(GamepadButton button = GamepadButton.South)
        {
            return new InputPropertyButton(
                new InputValueButtonGamepadRelease
                {
                    m_Button = button
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Gamepad.all.Count <= 0) return;
            if (!Gamepad.current[this.m_Button].wasReleasedThisFrame) return;
            
            this.ExecuteEventStart();
            this.ExecuteEventPerform();
        }
    }
}