using System;
using GameCreator.Runtime.Common;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Hover Enter")]
    [Category("UI/On Hover Enter")]
    [Description("Executed when the pointer hovers the UI element")]

    [Image(typeof(IconUIHoverEnter), ColorTheme.Type.Green)]
    
    [Keywords("Mouse", "Over", "Pointer")]

    [Serializable]
    public class EventUIOnHoverEnter : Event
    {
        public override Type RequiresComponent => typeof(Graphic);

        protected internal override void OnPointerEnter(Trigger trigger)
        {
            base.OnPointerEnter(trigger);
            if (!this.IsActive) return;
            
            _ = trigger.Execute(this.Self);
        }
    }
}