using System;

namespace GameCreator.Runtime.Common
{
    [Title("Touch While Pressing")]
    [Category("Mobile/Touch While Pressing")]
    
    [Description("While a finger is being held down on the touchscreen")]
    [Image(typeof(IconTouch), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Down", "Held", "Hold")]
    
    [Serializable]
    public class InputButtonTouchWhilePressing : TInputButtonTouch
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasTouchedThisFrame)
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
                new InputButtonTouchWhilePressing()
            );
        }
    }
}