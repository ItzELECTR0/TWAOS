using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Play Music")]
    [Description(
        "Plays a looped Audio Clip. Useful for background music or persistent sounds."
    )]

    [Category("Audio/Play Music")]
    
    [Parameter("Audio Clip", "The Audio Clip to be played")]
    [Parameter("Transition In", "Time it takes for the sound to fade in")]
    [Parameter("Spatial Blending", "Whether the sound is placed in a 3D space or not")]
    [Parameter("Target", "A Game Object reference that the sound follows as the source")]

    [Keywords("Audio", "Ambience", "Background")]
    [Image(typeof(IconHeadset), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonAudioMusicPlay : Instruction
    {
        [SerializeField] private PropertyGetAudio m_AudioClip = GetAudioClip.Create;
        [SerializeField] private AudioConfigMusic m_Config = new AudioConfigMusic();

        public override string Title => $"Play Music: {this.m_AudioClip}";

        protected override Task Run(Args args)
        {
            AudioClip audioClip = this.m_AudioClip.Get(args);
            if (audioClip == null) return DefaultResult;
            
            if (!AudioManager.Instance.Music.IsPlaying(audioClip))
            {
                _ = AudioManager.Instance.Music.Play(
                    audioClip, 
                    this.m_Config,
                    args
                );   
            }

            return DefaultResult;
        }
    }
}