using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Title("Gamepad Timeout")]
    [Category("Gamepad/Gamepad Timeout")]
    
    [Description("When a gamepad or joystick key is pressed and held for a certain amount of seconds")]
    [Image(typeof(IconGamepad), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    
    [Keywords("Key", "Button", "Timeout", "Delay", "Duration", "Hold")]
    
    [Serializable]
    public class InputValueButtonGamepadTimeout : TInputButton
    {
        private enum Mode
        {
            OnReleaseButton,
            OnTimeout
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GamepadButton m_Button = GamepadButton.South;
        [SerializeField] private Mode m_Mode = Mode.OnReleaseButton;
        [SerializeField] private float m_Duration = 0.5f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool IsFired { get; set; } = false;
        private float PressTime { get; set; } = -999f;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(GamepadButton button = GamepadButton.South)
        {
            return new InputPropertyButton(
                new InputValueButtonGamepadTimeout
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
                this.IsFired = false;
                this.PressTime = Time.unscaledTime;
                
                this.ExecuteEventStart();
            }
            
            if (this.m_Mode == Mode.OnTimeout && !this.IsFired)
            {
                if (Gamepad.current[this.m_Button].isPressed && this.IsTimeout())
                {
                    this.IsFired = true;
                    this.ExecuteEventPerform();
                }
            }
            
            if (Gamepad.current[this.m_Button].wasReleasedThisFrame)
            {
                if (this.IsFired) return;

                switch (this.m_Mode)
                {
                    case Mode.OnReleaseButton:
                        if (this.IsTimeout()) this.ExecuteEventPerform();
                        else this.ExecuteEventCancel();
                        break;
                    
                    case Mode.OnTimeout:
                        if (!this.IsFired) this.ExecuteEventCancel();
                        break;
                    
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private bool IsTimeout()
        {
            return Time.unscaledTime - this.PressTime > this.m_Duration;
        }
    }
}