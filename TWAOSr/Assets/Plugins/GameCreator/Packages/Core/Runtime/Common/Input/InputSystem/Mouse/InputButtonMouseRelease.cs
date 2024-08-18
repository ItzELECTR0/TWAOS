using System;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Release")]
    [Category("Mouse/Mouse Release")]
    
    [Description("When the specified mouse button is released")]
    [Image(typeof(IconMouse), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Keywords("Cursor", "Button", "Up")]
    
    [Serializable]
    public class InputButtonMouseRelease : TInputButtonMouse
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasReleasedThisFrame)
            {
                this.ExecuteEventStart();
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonMouseRelease()
            );
        }
    }
}