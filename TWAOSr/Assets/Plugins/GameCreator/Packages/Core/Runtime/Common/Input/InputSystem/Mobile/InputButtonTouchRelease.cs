using System;

namespace GameCreator.Runtime.Common
{
    [Title("Touch Release")]
    [Category("Mobile/Touch Release")]
    
    [Description("When a finer is released from the touchscreen")]
    [Image(typeof(IconTouch), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Keywords("Up")]
    
    [Serializable]
    public class InputButtonTouchRelease : TInputButtonTouch
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
                new InputButtonTouchRelease()
            );
        }
    }
}