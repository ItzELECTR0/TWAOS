using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Sound Effect Playing")]
    [Description("Returns true if the given sound effect is playing")]

    [Category("Audio/Is Sound Effect Playing")]
    
    [Parameter("Audio Clip", "The audio clip to check")]

    [Keywords("SFX", "Music", "Audio", "Running")]
    [Image(typeof(IconMusicNote), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class ConditionAudioIsPlaySoundEffect : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetAudio m_AudioClip = new PropertyGetAudio();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is SFX {this.m_AudioClip} playing";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            AudioClip audioClip = this.m_AudioClip.Get(args);
            return audioClip != null && AudioManager.Instance.SoundEffect.IsPlaying(audioClip);
        }
    }
}
