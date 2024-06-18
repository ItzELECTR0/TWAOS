using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Speech Playing")]
    [Description("Returns true if the given Speech sound is playing")]

    [Category("Audio/Is Speech Playing")]
    
    [Parameter("Audio Clip", "The audio clip to check")]

    [Keywords("SFX", "Music", "Audio", "Running")]
    [Image(typeof(IconFace), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class ConditionAudioIsPlaySpeech : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetAudio m_AudioClip = new PropertyGetAudio();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Speech {this.m_AudioClip} playing";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            AudioClip audioClip = this.m_AudioClip.Get(args);
            return audioClip != null && AudioManager.Instance.Speech.IsPlaying(audioClip);
        }
    }
}
