using UnityEngine;
using UnityEngine.Audio;


namespace GameCreator.Runtime.Common.Audio
{
    public class Ambient : TAudioChannel
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Volume => AudioManager.Instance.Volume.CurrentAmbient;

        protected override AudioMixerGroup AudioOutput =>
            Settings.From<GeneralRepository>().Audio.ambientMixer;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public Ambient(Transform parent) : base(parent)
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