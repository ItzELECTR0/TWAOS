using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Keyboard Timeout")]
    [Category("Keyboard/Keyboard Timeout")]
    
    [Description("When a keyboard key is pressed and held for a certain amount of seconds")]
    [Image(typeof(IconKey), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    
    [Keywords("Key", "Button", "Timeout", "Delay", "Duration", "Hold")]
    
    [Serializable]
    public class InputButtonKeyboardTimeout : TInputButton
    {
        private enum Mode
        {
            OnReleaseKey,
            OnTimeout
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Key m_Key = Key.Space;
        [SerializeField] private Mode m_Mode = Mode.OnReleaseKey;
        [SerializeField] private float m_Duration = 0.5f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool IsFired { get; set; } = false;
        private float PressTime { get; set; } = -999f;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(Key key = Key.Space)
        {
            return new InputPropertyButton(
                new InputButtonKeyboardTimeout
                {
                    m_Key = key
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Keyboard.current == null) return;
            
            if (Keyboard.current[this.m_Key].wasPressedThisFrame)
            {
                this.IsFired = false;
                this.PressTime = Time.unscaledTime;
                
                this.ExecuteEventStart();
            }
            
            if (this.m_Mode == Mode.OnTimeout && !this.IsFired)
            {
                if (Keyboard.current[this.m_Key].isPressed && this.IsTimeout())
                {
                    this.IsFired = true;
                    this.ExecuteEventPerform();
                }
            }
            
            if (Keyboard.current[this.m_Key].wasReleasedThisFrame)
            {
                if (this.IsFired) return;

                switch (this.m_Mode)
                {
                    case Mode.OnReleaseKey:
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