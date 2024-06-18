using System;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class GeneralAudio
    {
        public AudioMixerGroup soundEffectsMixer;
        public AudioMixerGroup ambientMixer;
        public AudioMixerGroup musicMixer;
        public AudioMixerGroup speechMixer;
        public AudioMixerGroup userInterfaceMixer;
    }
}