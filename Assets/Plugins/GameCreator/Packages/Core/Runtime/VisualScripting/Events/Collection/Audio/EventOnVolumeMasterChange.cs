using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change Master Volume")]
    [Category("Audio/On Change Master Volume")]
    [Description("Executed when the Master Volume is changed")]

    [Image(typeof(IconVolume), ColorTheme.Type.Blue)]
    
    [Keywords("Audio", "Sound", "Level")]

    [Serializable]
    public class EventOnVolumeMasterChange : Event
    {
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            AudioManager.Instance.Volume.EventMaster -= this.OnChange;
            AudioManager.Instance.Volume.EventMaster += this.OnChange;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            AudioManager.Instance.Volume.EventMaster -= this.OnChange;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}