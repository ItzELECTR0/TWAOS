using System;
using GameCreator.Runtime.Common;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Deselect")]
    [Category("UI/On Deselect")]
    [Description("Executed when the UI element is deselected")]

    [Image(typeof(IconRadioOff), ColorTheme.Type.Red)]
    
    [Keywords("Mouse", "Choose", "Focus", "Pick", "Pointer")]

    [Serializable]
    public class EventUIOnDeselect : Event
    {
        public override Type RequiresComponent => typeof(Selectable);

        protected internal override void OnDeselect(Trigger trigger)
        {
            base.OnDeselect(trigger);
            if (!this.IsActive) return;
            
            _ = trigger.Execute(this.Self);
        }
    }
}