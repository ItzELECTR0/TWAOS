using System;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Double Release")]
    [Category("Mouse/Mouse Double Release")]
    
    [Description("When the specified mouse button is released after a double press")]
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Keywords("Cursor", "Button", "Up", "Click")]
    
    [Serializable]
    public class InputButtonMouseDoubleRelease : TInputButtonMouse
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasReleasedThisFrame && this.PressCount == 2)
            {
                this.ExecuteEventStart();
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonMouseDoubleRelease()
            );
        }
    }
}