using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common
{
    [Title("Audio Mixer Parameter")]
    [Category("Audio/Audio Mixer Parameter")]
    
    [Image(typeof(IconAudioMixer), ColorTheme.Type.Yellow)]
    [Description("The specified Audio Mixer parameter value")]

    [Keywords("Audio", "Sound")]

    [Serializable]
    public class SetNumberAudioMixer : PropertyTypeSetNumber
    {
        [SerializeField] private AudioMixer m_AudioMixer;
        [SerializeField] private string m_Parameter;
        
        public override void Set(double value, Args args)
        {
            if (this.m_AudioMixer == null) return;
            this.m_AudioMixer.SetFloat(this.m_Parameter, (float) value);
        }

        public override double Get(Args args)
        {
            if (this.m_AudioMixer == null) return 0;
            return this.m_AudioMixer.GetFloat(this.m_Parameter, out float value)
                ? value
                : 0;
        }

        public override string String => string.Format(
            "{0}[{1}]", 
            this.m_AudioMixer != null ? this.m_AudioMixer.name : "(none)", 
            this.m_Parameter
        );
    }
}