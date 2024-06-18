using UnityEngine;
using UnityEngine.Audio;


namespace GameCreator.Runtime.Common.Audio
{
    public class Music : TAudioChannel
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Volume => AudioManager.Instance.Volume.CurrentMusic;

        protected override AudioMixerGroup AudioOutput =>
            Settings.From<GeneralRepository>().Audio.musicMixer;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public Music(Transform parent) : base(parent)
        { }
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override AudioBuffer MakeAudioBuffer()
        {
            AudioBuffer audioBuffer = base.MakeAudioBuffer();
            audioBuffer.AudioSource.loop = true;

            return audioBuffer;
        }
    }
}