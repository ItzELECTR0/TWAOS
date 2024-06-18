using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Snapshot")]
    [Description("Smoothly transitions to a new snapshot over a period of time")]

    [Category("Audio/Change Snapshot")]

    [Parameter("Snapshot", "The Audio Mixer Snapshot that is activated")]
    [Parameter("Transition", "How long it takes to transition to the new Snapshot")]

    [Keywords("Effect", "Transition", "Effect", "Change")]
    [Image(typeof(IconAudioMixer), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonAudioSnapshot : Instruction
    {
        [SerializeField] private AudioMixerSnapshot m_Snapshot;
        
        [SerializeField] private PropertyGetDecimal m_Transition = new PropertyGetDecimal(0.5f);

        public override string Title => string.Format(
            "Change to Snapshot {0} in {1} seconds",
            this.m_Snapshot != null ? this.m_Snapshot.name : "(none)",
            this.m_Transition
        );

        protected override Task Run(Args args)
        {
            if (this.m_Snapshot != null)
            {
                float duration = (float) this.m_Transition.Get(args);
                this.m_Snapshot.TransitionTo(duration);
            }

            return DefaultResult;
        }
    }
}