using System;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse While Pressing")]
    [Category("Mouse/Mouse While Pressing")]
    
    [Description("While the specified mouse button is being held down")]
    [Image(typeof(IconMouse), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Cursor", "Button", "Down", "Held", "Hold")]
    
    [Serializable]
    public class InputButtonMouseWhilePressing : TInputButtonMouse
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasPressedThisFrame)
            {
                this.ExecuteEventStart();   
            }
            
            if (this.IsPressed)
            {
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonMouseWhilePressing()
            );
        }
    }
}