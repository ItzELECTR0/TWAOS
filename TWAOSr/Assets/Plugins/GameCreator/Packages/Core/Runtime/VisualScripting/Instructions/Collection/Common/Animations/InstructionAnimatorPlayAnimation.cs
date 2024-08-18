using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Play Animation Clip")]
    [Description("Plays an Animation Clip on the chosen Animator")]
    
    [Image(typeof(IconPlayCircle), ColorTheme.Type.Blue)]

    [Category("Animations/Play Animation Clip")]
    
    [Parameter("Animation Clip", "The Animation Clip that is played")]

    [Keywords("Animate", "Reproduce", "Sequence", "Cinematic")]
    [Serializable]
    public class InstructionAnimatorPlayAnimation : TInstructionAnimator
    {
        [SerializeField] private PropertyGetAnimation m_AnimationClip = GetAnimationInstance.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Play {this.m_AnimationClip} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return DefaultResult;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return DefaultResult;
            
            AnimationClip animationClip = this.m_AnimationClip.Get(args);
            
            AnimationPlayableUtilities.PlayClip(animator, animationClip, out PlayableGraph _);
            return DefaultResult;
        }
    }
}