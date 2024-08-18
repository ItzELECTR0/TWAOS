using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On App Quit")]
    [Category("Lifecycle/On App Quit")]
    [Description("Executed right before exiting the standalone application")]

    [Image(typeof(IconExit), ColorTheme.Type.Red)]
    
    [Keywords("Exit", "Close")]

    [Serializable]
    public class EventOnAppQuit : Event
    {
        protected internal override void OnApplicationQuit(Trigger trigger)
        {
            base.OnApplicationQuit(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}