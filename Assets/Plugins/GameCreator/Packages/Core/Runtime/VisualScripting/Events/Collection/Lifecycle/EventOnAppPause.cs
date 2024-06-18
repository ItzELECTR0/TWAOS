using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On App Pause")]
    [Category("Lifecycle/On App Pause")]
    [Description("Executed when the standalone application loses its focus")]

    [Image(typeof(IconSquareOutline), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Keywords("Background", "Suspend")]

    [Serializable]
    public class EventOnAppPause : Event
    {
        protected internal override void OnApplicationPause(Trigger trigger, bool hasFocus)
        {
            base.OnApplicationPause(trigger, hasFocus);
            _ = trigger.Execute(this.Self);
        }
    }
}