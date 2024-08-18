using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Start")]
    [Category("Lifecycle/On Start")]
    [Description("Executed on the frame when the game object is enabled for the first time")]

    [Image(typeof(IconArrowCircleRight), ColorTheme.Type.Yellow)]
    
    [Keywords("Initialize")]

    [Serializable]
    public class EventOnStart : Event
    {
        protected internal override void OnStart(Trigger trigger)
        {
            base.OnStart(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}