using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Fade all Music")]
    [Description("Stops all Music currently playing")]

    [Category("Audio/Fade all Music")]
    
    [Parameter("Wait To Complete", "Check if you want to wait until the sound has faded out")]
    [Parameter("Transition Out", "Time it takes for the sound to fade out")]

    [Keywords("Audio", "Music", "Background", "Fade", "Mute")]
    [Image(typeof(IconHeadset), ColorTheme.Type.TextLight, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionCommonAudioMusicStopAll : Instruction
    {
        [SerializeField] private bool m_WaitToComplete;
        [SerializeField] private float transitionOut = 2f;

        public override string Title => string.Format(
            "Stop all Music {0}",
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
            if (this.m_WaitToComplete)
            {
                await AudioManager.Instance.Music.StopAll(this.transitionOut);
            }
            else
            {
                _ = AudioManager.Instance.Music.StopAll(this.transitionOut);
            }
        }
    }
}