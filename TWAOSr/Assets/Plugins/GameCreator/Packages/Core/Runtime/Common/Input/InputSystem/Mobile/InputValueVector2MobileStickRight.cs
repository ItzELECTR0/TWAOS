using System;

namespace GameCreator.Runtime.Common
{
    [Title("Right Stick")]
    [Category("Mobile/Right Stick")]
    
    [Description("")]
    [Image(typeof(IconTouchstick), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Keywords("Virtual", "Joystick", "Touchstick", "Direction")]
    
    [Serializable]
    public class InputValueVector2MobileStickRight : TInputValueVector2MobileStick
    {
        protected override ITouchStick CreateTouchStick()
        {
            return TouchStickRight.Create();
        }

        public static InputPropertyValueVector2 Create => new InputPropertyValueVector2(
            new InputValueVector2MobileStickRight()
        );
    }
}