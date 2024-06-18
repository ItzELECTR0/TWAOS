using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Ambient Playing")]
    [Description("Returns true if the given Ambient sound is playing")]

    [Category("Audio/Is Ambient Playing")]
    
    [Parameter("Audio Clip", "The audio clip to check")]

    [Keywords("SFX", "Music", "Audio", "Running")]
    [Image(typeof(IconBird), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class ConditionAudioIsPlayAmbient : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetAudio m_AudioClip = new PropertyGetAudio();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Ambient {this.m_AudioClip} playing";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            AudioClip audioClip = this.m_AudioClip.Get(args);
            return audioClip != null && AudioManager.Instance.Ambient.IsPlaying(audioClip);
        }
    }
}
