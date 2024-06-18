using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Music")]
    [Description("Stops a currently playing Music audio")]

    [Category("Audio/Stop Music")]
    
    [Parameter("Audio Clip", "The Audio Clip to be played")]
    [Parameter("Wait To Complete", "Check if you want to wait until the sound has faded out")]
    [Parameter("Transition Out", "Time it takes for the sound to fade out")]

    [Keywords("Audio", "Music", "Background", "Fade", "Mute")]
    [Image(typeof(IconHeadset), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCommonAudioMusicStop : Instruction
    {
        [SerializeField] private PropertyGetAudio m_AudioClip = GetAudioClip.Create;
        
        [SerializeField] private bool m_WaitToComplete = false;
        [SerializeField] private float transitionOut = 2f;

        public override string Title => string.Format(
            "Stop Music: {0} {1}",
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
                await AudioManager.Instance.Music.Stop(
                    audioClip,
                    this.transitionOut
                );
            }
            else
            {
                _ = AudioManager.Instance.Music.Stop(
                    audioClip,
                    this.transitionOut
                );
            }
        }
    }
}