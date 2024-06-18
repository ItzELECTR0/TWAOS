using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common.Audio
{
    public class Speech : TAudioChannel
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Volume => AudioManager.Instance.Volume.CurrentSpeech;
        
        protected override AudioMixerGroup AudioOutput =>
            Settings.From<GeneralRepository>().Audio.speechMixer;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public Speech(Transform parent) : base(parent)
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