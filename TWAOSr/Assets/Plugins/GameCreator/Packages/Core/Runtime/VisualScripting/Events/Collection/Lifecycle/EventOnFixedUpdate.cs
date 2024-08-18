using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Fixed Update")]
    [Category("Lifecycle/On Fixed Update")]
    [Description("Executed every fixed frame as long as the game object is enabled (physics loop")]

    [Image(typeof(IconLoop), ColorTheme.Type.Green)]

    [Keywords("Loop", "Tick", "Continuous", "Physics", "Rigidbody")]

    [Serializable]
    public class EventOnFixedUpdate : Event
    {
        protected internal override void OnFixedUpdate(Trigger trigger)
        {
            base.OnFixedUpdate(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}