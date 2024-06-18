using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change UI Volume")]
    [Category("Audio/On Change UI Volume")]
    [Description("Executed when the UI Volume is changed")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Keywords("Audio", "Sound", "Level")]

    [Serializable]
    public class EventOnVolumeUIChange : Event
    {
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            AudioManager.Instance.Volume.EventUI -= this.OnChange;
            AudioManager.Instance.Volume.EventUI += this.OnChange;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            AudioManager.Instance.Volume.EventUI -= this.OnChange;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}