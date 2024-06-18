using System;
using GameCreator.Runtime.Common;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Mouse Held Down")]
    [Description("Returns true if the mouse button is being held down")]

    [Category("Input/Is Mouse Held Down")]

    [Keywords("Key", "Up", "Click")]
    
    [Image(typeof(IconMouse), ColorTheme.Type.Blue, typeof(OverlayDot))]
    [Serializable]
    public class ConditionInputMouseHeldDown : TConditionMouse
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Mouse {this.m_Button} held down";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return false;

            return this.m_Button switch
            {
                Button.Left => mouse.leftButton.isPressed,
                Button.Right => mouse.rightButton.isPressed,
                Button.Middle => mouse.middleButton.isPressed,
                Button.Forward => mouse.forwardButton.isPressed,
                Button.Back => mouse.backButton.isPressed,
                _ => throw new ArgumentOutOfRangeException($"Mouse '{this.m_Button}' not found")
            };
        }
    }
}
