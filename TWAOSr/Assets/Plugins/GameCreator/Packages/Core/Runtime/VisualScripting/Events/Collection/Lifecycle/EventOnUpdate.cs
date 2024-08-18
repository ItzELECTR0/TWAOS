using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Update")]
    [Category("Lifecycle/On Update")]
    [Description("Executed every frame as long as the game object is enabled")]

    [Image(typeof(IconLoop), ColorTheme.Type.Blue)]
    
    [Keywords("Loop", "Tick", "Continuous")]

    [Serializable]
    public class EventOnUpdate : Event
    {
        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}