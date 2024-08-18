using System;
using GameCreator.Runtime.Common;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Mouse Pressed")]
    [Description("Returns true if the mouse button is pressed during this frame")]

    [Category("Input/Is Mouse Pressed")]

    [Keywords("Key", "Down")]
    
    [Image(typeof(IconMouse), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    [Serializable]
    public class ConditionInputMousePress : TConditionMouse
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Mouse {this.m_Button} just pressed";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return false;

            return this.m_Button switch
            {
                Button.Left => mouse.leftButton.wasPressedThisFrame,
                Button.Right => mouse.rightButton.wasPressedThisFrame,
                Button.Middle => mouse.middleButton.wasPressedThisFrame,
                Button.Forward => mouse.forwardButton.wasPressedThisFrame,
                Button.Back => mouse.backButton.wasPressedThisFrame,
                _ => throw new ArgumentOutOfRangeException($"Mouse '{this.m_Button}' not found")
            };
        }
    }
}
