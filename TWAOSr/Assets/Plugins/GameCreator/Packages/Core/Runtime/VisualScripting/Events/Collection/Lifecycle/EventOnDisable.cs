using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Disable")]
    [Category("Lifecycle/On Disable")]
    [Description("Executed when the game object it is attached to becomes disabled or inactive")]

    [Image(typeof(IconRadioOff), ColorTheme.Type.Red)]
    
    [Keywords("Inactive", "Active", "Enable")]

    [Serializable]
    public class EventOnDisable : Event
    {
        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}