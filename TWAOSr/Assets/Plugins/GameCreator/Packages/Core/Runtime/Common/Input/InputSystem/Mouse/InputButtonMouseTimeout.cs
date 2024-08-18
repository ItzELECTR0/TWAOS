using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Timeout")]
    [Category("Mouse/Mouse Timeout")]
    
    [Description("When a mouse button is pressed and held for a certain amount of seconds")]
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Keywords("Timeout", "Delay", "Duration", "Hold")]
    
    [Serializable]
    public class InputButtonMouseTimeout : TInputButtonMouse
    {
        private enum Mode
        {
            OnReleaseMouse,
            OnTimeout
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private Mode m_Mode = Mode.OnReleaseMouse;
        [SerializeField] private float m_Duration = 0.5f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool IsFired { get; set; } = false;
        private float PressTime { get; set; } = -999f;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(MouseButton button = MouseButton.Left)
        {
            return new InputPropertyButton(
                new InputButtonMouseTimeout
                {
                    m_Button = button,
                    m_Mode = Mode.OnReleaseMouse,
                    m_Duration = 0.5f
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (this.WasPressedThisFrame)
            {
                this.IsFired = false;
                this.PressTime = Time.unscaledTime;
                
                this.ExecuteEventStart();
            }
            
            if (this.m_Mode == Mode.OnTimeout && !this.IsFired)
            {
                if (this.IsPressed && this.IsTimeout())
                {
                    this.IsFired = true;
                    this.ExecuteEventPerform();
                }
            }
            
            if (this.WasReleasedThisFrame)
            {
                if (this.IsFired) return;

                switch (this.m_Mode)
                {
                    case Mode.OnReleaseMouse:
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