using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Become Invisible")]
    [Category("Lifecycle/On Become Invisible")]
    [Description("Executed when the game object it is attached to is no longer visible by any camera")]

    [Image(typeof(IconEye), ColorTheme.Type.Red)]
    
    [Keywords("Hide", "Disappear")]

    [Serializable]
    public class EventOnBecomeInvisible : Event
    {
        protected internal override void OnBecameInvisible(Trigger trigger)
        {
            base.OnBecameInvisible(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}