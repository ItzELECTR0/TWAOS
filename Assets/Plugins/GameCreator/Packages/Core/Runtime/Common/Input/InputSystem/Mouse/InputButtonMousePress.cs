using System;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Press")]
    [Category("Mouse/Mouse Press")]
    
    [Description("When the specified mouse button is pressed")]
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    
    [Keywords("Cursor", "Button", "Down")]
    
    [Serializable]
    public class InputButtonMousePress : TInputButtonMouse
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasPressedThisFrame)
            {
                this.ExecuteEventStart();
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonMousePress()
            );
        }
    }
}