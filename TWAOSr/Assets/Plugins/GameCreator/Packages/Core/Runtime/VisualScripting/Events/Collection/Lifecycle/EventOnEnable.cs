using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Enable")]
    [Category("Lifecycle/On Enable")]
    [Description("Executed when the game object it is attached to becomes enabled and active")]

    [Image(typeof(IconRadioOn), ColorTheme.Type.Yellow)]
    
    [Keywords("Active", "Disable", "Inactive")]

    [Serializable]
    public class EventOnEnable : Event
    {
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}