using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Play Gesture")]
    [Description("Plays an Animation Clip on a Character once")]

    [Category("Characters/Animation/Play Gesture")]

    [Parameter("Character", "The character that plays the animation")]
    [Parameter("Animation Clip", "The Animation Clip that is played")]
    [Parameter(
        "Avatar Mask",
         "(Optional) Allows to play the animation on specific body parts of the Character"
    )]
    [Parameter(
        "Blend Mode",
        "Additively adds the new animation on top of the rest or overrides any lower layer animations"
    )]
    
    [Parameter("Delay", "Amount of seconds to wait before the animation starts to play")]
    [Parameter("Speed", "Speed coefficient at which the animation plays. 1 means normal speed")]
    [Parameter("Transition In", "The amount of seconds the animation takes to blend in")]
    [Parameter("Transition Out", "The amount of seconds the animation takes to blend out")]
    
    [Parameter("Wait To Complete", "If true this Instruction waits until the animation is complete")]

    [Keywords("Characters", "Animation", "Animate", "Gesture", "Play")]
    [Image(typeof(IconCharacterGesture), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCharacterGesture : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space]
        [SerializeField] private PropertyGetAnimation m_AnimationClip = GetAnimationInstance.Create;
        [SerializeField] private AvatarMask m_AvatarMask = null;
        [SerializeField] private BlendMode m_BlendMode = BlendMode.Blend;

        [SerializeField] private PropertyGetDecimal m_Delay = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Speed = GetDecimalConstantOne.Create;
        [SerializeField] private bool m_UseRootMotion = false;
        [SerializeField] private PropertyGetDecimal m_TransitionIn = new PropertyGetDecimal(0.1f);
        [SerializeField] private PropertyGetDecimal m_TransitionOut = new PropertyGetDecimal(0.1f);

        [Space] 
        [SerializeField] private bool m_WaitToComplete = true;

        public override string Title => $"Gesture {this.m_AnimationClip} on {this.m_Character}";

        protected override async Task Run(Args args)
        {
            AnimationClip animationClip = this.m_AnimationClip.Get(args);
            if (animationClip == null) return;
            
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;
            
            ConfigGesture configuration = new ConfigGesture(
                (float) this.m_Delay.Get(args), animationClip.length, 
                (float) this.m_Speed.Get(args), this.m_UseRootMotion,
                (float) this.m_TransitionIn.Get(args),
                (float) this.m_TransitionOut.Get(args)
            );
            
            Task gestureTask = character.Gestures.CrossFade(
                animationClip, this.m_AvatarMask, this.m_BlendMode,
                configuration, false
            );

            if (this.m_WaitToComplete) await gestureTask;
        }
    }
}