using System;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Double Press")]
    [Category("Mouse/Mouse Double Press")]
    
    [Description("When the specified mouse button is pressed twice in a row")]
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    
    [Keywords("Cursor", "Button", "Down", "Click")]
    
    [Serializable]
    public class InputButtonMouseDoublePress : TInputButtonMouse
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasPressedThisFrame && this.PressCount == 2)
            {
                this.ExecuteEventStart();
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonMouseDoublePress()
            );
        }
    }
}