using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On App Focus")]
    [Category("Lifecycle/On App Focus")]
    [Description("Executed when the standalone application is brought to focus")]

    [Image(typeof(IconSquareOutline), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    
    [Keywords("Foreground")]

    [Serializable]
    public class EventOnAppFocus : Event
    {
        protected internal override void OnApplicationFocus(Trigger trigger, bool hasFocus)
        {
            base.OnApplicationFocus(trigger, hasFocus);
            _ = trigger.Execute(this.Self);
        }
    }
}