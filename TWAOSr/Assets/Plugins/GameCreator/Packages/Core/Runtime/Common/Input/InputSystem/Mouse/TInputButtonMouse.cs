using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.Common
{
    [Keywords("Key", "Button", "Left", "Right", "Middle")]
    
    [Serializable]
    public abstract class TInputButtonMouse : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected MouseButton m_Button = MouseButton.Left;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected bool WasPressedThisFrame
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().wasPressedThisFrame;
            }
        }
        
        protected bool WasReleasedThisFrame
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().wasReleasedThisFrame;
            }
        }
        
        protected bool IsPressed
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().IsPressed();
            }
        }

        protected int PressCount => Mouse.current != null 
            ? Mouse.current.clickCount.ReadValue() 
            : 0;

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private ButtonControl GetButton()
        {
            return this.m_Button switch
            {
                MouseButton.Left => Mouse.current.leftButton,
                MouseButton.Right => Mouse.current.rightButton,
                MouseButton.Middle => Mouse.current.middleButton,
                MouseButton.Forward => Mouse.current.forwardButton,
                MouseButton.Back => Mouse.current.backButton,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}