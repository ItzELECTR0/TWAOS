using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Become Visible")]
    [Category("Lifecycle/On Become Visible")]
    [Description("Executed when the game object it is attached to becomes visible to any camera")]

    [Image(typeof(IconEye), ColorTheme.Type.Yellow)]
    
    [Keywords("Show", "Render", "Appear")]

    [Serializable]
    public class EventOnBecomeVisible : Event
    {
        protected internal override void OnBecameVisible(Trigger trigger)
        {
            base.OnBecameVisible(trigger);
            _ = trigger.Execute(this.Self);
        }
    }
}