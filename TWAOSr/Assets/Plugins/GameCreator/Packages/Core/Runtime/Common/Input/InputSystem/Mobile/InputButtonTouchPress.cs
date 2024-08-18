using System;

namespace GameCreator.Runtime.Common
{
    [Title("Touch Press")]
    [Category("Mobile/Touch Press")]
    
    [Description("When a finger touches the touchscreen")]
    [Image(typeof(IconTouch), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    
    [Keywords("Down")]
    
    [Serializable]
    public class InputButtonTouchPress : TInputButtonTouch
    {
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.WasTouchedThisFrame)
            {
                this.ExecuteEventStart();
                this.ExecuteEventPerform();
            }
        }

        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonTouchPress()
            );
        }
    }
}