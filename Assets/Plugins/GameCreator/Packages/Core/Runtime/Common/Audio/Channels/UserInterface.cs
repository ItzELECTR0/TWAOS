using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common.Audio
{
    public class UserInterface : TAudioChannel
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Volume => AudioManager.Instance.Volume.CurrentUI;
        
        protected override AudioMixerGroup AudioOutput =>
            Settings.From<GeneralRepository>().Audio.userInterfaceMixer;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public UserInterface(Transform parent) : base(parent)
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