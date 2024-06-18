using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Audio Mixer Parameter")]
    [Description("Changes the value of an Audio Mixer exposed parameter")]

    [Category("Audio/Audio Mixer Parameter")]

    [Parameter("Audio Mixer", "The Audio Mixer asset with the exposed parameter")]
    [Parameter("Parameter Name", "A string representing the name of the exposed parameter")]
    [Parameter("Parameter Value", "The value which the exposed parameter is set")]

    [Keywords("Float", "Exposed", "Effect", "Change")]
    [Image(typeof(IconAudioMixer), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonAudioMixerParameter : Instruction
    {
        [SerializeField] private AudioMixer m_AudioMixer;
        
        [SerializeField] private PropertyGetString m_ParameterName = new PropertyGetString("Parameter_Name");
        [SerializeField] private PropertyGetDecimal m_ParameterValue = new PropertyGetDecimal(1f);

        public override string Title => string.Format(
            "Audio Mixer {0} set '{1}' = {2}",
            this.m_AudioMixer != null ? this.m_AudioMixer.name : "(none)",
            this.m_ParameterName,
            this.m_ParameterValue
        );

        protected override Task Run(Args args)
        {
            if (this.m_AudioMixer != null)
            {
                this.m_AudioMixer.SetFloat(
                    this.m_ParameterName.Get(args),
                    (float) this.m_ParameterValue.Get(args)
                );
            }

            return DefaultResult;
        }
    }
}