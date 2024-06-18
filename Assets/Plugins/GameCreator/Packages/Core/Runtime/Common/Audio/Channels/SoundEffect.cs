using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common.Audio
{
    public class SoundEffect : TAudioChannel
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Volume => AudioManager.Instance.Volume.CurrentSoundEffects;
        
        protected override AudioMixerGroup AudioOutput =>
            Settings.From<GeneralRepository>().Audio.soundEffectsMixer;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public SoundEffect(Transform parent) : base(parent)
        { }
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override AudioBuffer MakeAudioBuffer()
        {
            AudioBuffer audioBuffer = base.MakeAudioBuffer();
            audioBuffer.AudioSource.loop = false;
            
            return audioBuffer;
        }
    }
}