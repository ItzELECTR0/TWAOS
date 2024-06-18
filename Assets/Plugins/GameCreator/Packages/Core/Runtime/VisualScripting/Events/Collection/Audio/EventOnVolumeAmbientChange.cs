using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change Ambient Volume")]
    [Category("Audio/On Change Ambient Volume")]
    [Description("Executed when the Ambient Volume is changed")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Keywords("Audio", "Sound", "Level")]

    [Serializable]
    public class EventOnVolumeAmbientChange : Event
    {
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            AudioManager.Instance.Volume.EventAmbient -= this.OnChange;
            AudioManager.Instance.Volume.EventAmbient += this.OnChange;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            AudioManager.Instance.Volume.EventAmbient -= this.OnChange;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}