using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad Press")]
    [Category("Gamepad/Gamepad Press")]
    
    [Description("When a gamepad or joystick key is pressed")]
    [Image(typeof(IconGamepad), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    
    [Keywords("Key", "Button", "Down", "Joystick")]
    
    [Serializable]
    public class InputValueButtonGamepadPress : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GamepadButton m_Button = GamepadButton.South;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(GamepadButton button = GamepadButton.South)
        {
            return new InputPropertyButton(
                new InputValueButtonGamepadPress
                {
                    m_Button = button
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Gamepad.all.Count <= 0) return;
            if (!Gamepad.current[this.m_Button].wasPressedThisFrame) return;
            
            this.ExecuteEventStart();
            this.ExecuteEventPerform();
        }
    }
}