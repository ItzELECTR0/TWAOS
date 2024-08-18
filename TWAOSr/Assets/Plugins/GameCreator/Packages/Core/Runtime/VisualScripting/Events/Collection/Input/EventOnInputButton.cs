using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Input Button")]
    [Category("Input/On Input Button")]
    [Description("Detects when a button is interacted with")]
    
    [Image(typeof(IconButton), ColorTheme.Type.Yellow)]
    [Keywords("Down", "Up", "Press", "Release")]
    [Keywords("Keyboard", "Mouse", "Button", "Gamepad", "Controller", "Joystick")]
    
    [Serializable]
    public class EventOnInputButton : TEventButton
    {
        protected override void OnInput()
        {
            base.OnInput();
            this.Execute();
        }
    }
}