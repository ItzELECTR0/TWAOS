using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change Speech Volume")]
    [Category("Audio/On Change Speech Volume")]
    [Description("Executed when the Speech Volume is changed")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Keywords("Audio", "Sound", "Level")]

    [Serializable]
    public class EventOnVolumeSpeechChange : Event
    {
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            AudioManager.Instance.Volume.EventSpeech -= this.OnChange;
            AudioManager.Instance.Volume.EventSpeech += this.OnChange;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            AudioManager.Instance.Volume.EventSpeech -= this.OnChange;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}