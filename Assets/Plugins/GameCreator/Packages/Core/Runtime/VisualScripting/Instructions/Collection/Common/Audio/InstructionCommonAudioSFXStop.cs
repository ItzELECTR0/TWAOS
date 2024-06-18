using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Sound Effect")]
    [Description("Stops a currently playing Sound Effect")]

    [Category("Audio/Stop Sound Effect")]

    [Keywords("Audio", "Sounds", "Silence", "Fade", "Mute", "SFX", "FX")]
    [Image(typeof(IconMusicNote), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCommonAudioSFXStop : Instruction
    {
        [SerializeField] private PropertyGetAudio m_AudioClip = GetAudioClip.Create;
        
        [SerializeField] private bool m_WaitToComplete;
        [SerializeField] private float transitionOut = 0.1f;

        public override string Title => string.Format(
            "Stop SFX: {0} {1}",
            this.m_AudioClip,
            this.transitionOut < float.Epsilon 
                ? string.Empty 
                : string.Format(
                    "in {0} second{1}", 
                    this.transitionOut,
                    Mathf.Approximately(this.transitionOut, 1f) ? string.Empty : "s"
                )
        );

        protected override async Task Run(Args args)
        {
            AudioClip audioClip = this.m_AudioClip.Get(args);
            if (audioClip == null) return;
            
            if (this.m_WaitToComplete)
            {
                await AudioManager.Instance.SoundEffect.Stop(
                    audioClip,
                    this.transitionOut
                );
            }
            else
            {
                _ = AudioManager.Instance.SoundEffect.Stop(
                    audioClip,
                    this.transitionOut
                );
            }
        }
    }
}