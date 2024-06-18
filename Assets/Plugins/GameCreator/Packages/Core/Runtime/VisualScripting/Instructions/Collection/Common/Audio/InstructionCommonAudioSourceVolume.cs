using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Audio Source Volume")]
    [Description("Changes the volume of an Audio Source component")]

    [Category("Audio/Audio Source Volume")]
    
    [Parameter("Audio Source", "The Audio Source component")]
    [Parameter("Volume", "The new targeted volume to change")]
    [Parameter("Transition", "How long it takes to reach the new value")]

    [Keywords("Clip", "Music")]
    [Image(typeof(IconAudioSource), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonAudioSourceVolume : Instruction
    {
        [SerializeField]
        private PropertyGetGameObject m_AudioSource = GetGameObjectInstance.Create();

        [SerializeField] private PropertyGetDecimal m_Volume = GetDecimalDecimal.Create(1f);
        [SerializeField] private Transition m_Transition = new Transition();

        public override string Title => $"Volume of {this.m_AudioSource} = {this.m_Volume}";

        protected override async Task Run(Args args)
        {
            AudioSource audioSource = this.m_AudioSource.Get<AudioSource>(args);
            if (audioSource == null) return;

            float valueSource = audioSource.volume;
            float valueTarget = (float) this.m_Volume.Get(args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => audioSource.volume = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(AudioSource), "volume"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(audioSource.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}