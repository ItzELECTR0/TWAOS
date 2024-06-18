using System;

namespace GameCreator.Runtime.Common
{
    [Title("Left Stick")]
    [Category("Mobile/Left Stick")]
    
    [Description("")]
    [Image(typeof(IconTouchstick), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    
    [Keywords("Virtual", "Joystick", "Touchstick", "Direction")]
    
    [Serializable]
    public class InputValueVector2MobileStickLeft : TInputValueVector2MobileStick
    {
        protected override ITouchStick CreateTouchStick()
        {
            return TouchStickLeft.Create();
        }
        
        public static InputPropertyValueVector2 Create => new InputPropertyValueVector2(
            new InputValueVector2MobileStickLeft()
        );
    }
}