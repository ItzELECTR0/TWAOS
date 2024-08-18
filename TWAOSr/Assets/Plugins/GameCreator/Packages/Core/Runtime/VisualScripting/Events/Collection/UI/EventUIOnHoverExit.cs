using System;
using GameCreator.Runtime.Common;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Hover Exit")]
    [Category("UI/On Hover Exit")]
    [Description("Executed when the pointer exits the hovered UI element")]

    [Image(typeof(IconUIHoverExit), ColorTheme.Type.Red)]
    
    [Keywords("Mouse", "Over", "Pointer")]

    [Serializable]
    public class EventUIOnHoverExit : Event
    {
        public override Type RequiresComponent => typeof(Graphic);
        
        protected internal override void OnPointerExit(Trigger trigger)
        {
            base.OnPointerExit(trigger);
            if (!this.IsActive) return;
            
            _ = trigger.Execute(this.Self);
        }
    }
}