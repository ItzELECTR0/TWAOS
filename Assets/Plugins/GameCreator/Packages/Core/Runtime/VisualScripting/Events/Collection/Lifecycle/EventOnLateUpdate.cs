using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Late Update")]
    [Category("Lifecycle/On Late Update")]
    [Description("Executed every frame after all On Update events are fired, as long as the game object is enabled")]

    [Image(typeof(IconLoop), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    
    [Keywords("Loop", "Tick", "Continuous")]

    [Serializable]
    public class EventOnLateUpdate : Event
    {
        protected internal override void OnLateUpdate(Trigger trigger)
        {
            base.OnLateUpdate(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}