using System;
using GameCreator.Runtime.Common;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Mouse Released")]
    [Description("Returns true if the mouse button is released during this frame")]

    [Category("Input/Is Mouse Released")]

    [Keywords("Key", "Up", "Click")]
    
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Serializable]
    public class ConditionInputMouseRelease : TConditionMouse
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Mouse {this.m_Button} just released";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return false;

            return this.m_Button switch
            {
                Button.Left => mouse.leftButton.wasReleasedThisFrame,
                Button.Right => mouse.rightButton.wasReleasedThisFrame,
                Button.Middle => mouse.middleButton.wasReleasedThisFrame,
                Button.Forward => mouse.forwardButton.wasReleasedThisFrame,
                Button.Back => mouse.backButton.wasReleasedThisFrame,
                _ => throw new ArgumentOutOfRangeException($"Mouse '{this.m_Button}' not found")
            };
        }
    }
}
