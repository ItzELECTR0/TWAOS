using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Fade all Ambient")]
    [Description("Stops all Ambient currently playing")]

    [Category("Audio/Fade all Ambient")]
    
    [Parameter("Wait To Complete", "Check if you want to wait until the sound has faded out")]
    [Parameter("Transition Out", "Time it takes for the sound to fade out")]

    [Keywords("Audio", "Ambience", "Background", "Fade", "Mute")]
    [Image(typeof(IconBird), ColorTheme.Type.TextLight, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionCommonAudioAmbientStopAll : Instruction
    {
        [SerializeField] private bool m_WaitToComplete;
        [SerializeField] private float transitionOut = 2f;

        public override string Title => string.Format(
            "Stop Ambient: {0}",
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
                await AudioManager.Instance.Ambient.StopAll(this.transitionOut);
            }
            else
            {
                _ = AudioManager.Instance.Ambient.StopAll(this.transitionOut);
            }
        }
    }
}