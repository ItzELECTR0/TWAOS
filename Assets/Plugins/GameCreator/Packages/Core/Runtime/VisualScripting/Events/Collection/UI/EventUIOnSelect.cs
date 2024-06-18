using System;
using GameCreator.Runtime.Common;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Select")]
    [Category("UI/On Select")]
    [Description("Executed when the UI element is selected")]

    [Image(typeof(IconRadioOn), ColorTheme.Type.Green)]
    
    [Keywords("Mouse", "Choose", "Focus", "Pick", "Pointer")]

    [Serializable]
    public class EventUIOnSelect : Event
    {
        public override Type RequiresComponent => typeof(Selectable);

        protected internal override void OnSelect(Trigger trigger)
        {
            base.OnSelect(trigger);
            if (!this.IsActive) return;
            
            _ = trigger.Execute(this.Self);
        }
    }
}