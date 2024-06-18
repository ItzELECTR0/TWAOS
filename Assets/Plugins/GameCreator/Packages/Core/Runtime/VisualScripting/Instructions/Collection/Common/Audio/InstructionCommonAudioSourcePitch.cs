using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Audio Source Pitch")]
    [Description("Changes the pitch of an Audio Source component")]

    [Category("Audio/Audio Source Pitch")]
    
    [Parameter("Audio Source", "The Audio Source component")]
    [Parameter("Pitch", "The new targeted pitch to change")]
    [Parameter("Transition", "How long it takes to reach the new value")]

    [Keywords("Clip", "Music")]
    [Image(typeof(IconAudioSource), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonAudioSourcePitch : Instruction
    {
        [SerializeField]
        private PropertyGetGameObject m_AudioSource = GetGameObjectInstance.Create();

        [SerializeField] private PropertyGetDecimal m_Pitch = GetDecimalDecimal.Create(1f);
        [SerializeField] private Transition m_Transition = new Transition();

        public override string Title => $"Pitch of {this.m_AudioSource} = {this.m_Pitch}";

        protected override async Task Run(Args args)
        {
            AudioSource audioSource = this.m_AudioSource.Get<AudioSource>(args);
            if (audioSource == null) return;

            float valueSource = audioSource.pitch;
            float valueTarget = (float) this.m_Pitch.Get(args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => audioSource.pitch = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(AudioSource), "pitch"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(audioSource.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}