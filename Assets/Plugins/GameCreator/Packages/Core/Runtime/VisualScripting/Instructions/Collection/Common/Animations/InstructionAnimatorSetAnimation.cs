using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Animation")]
    [Description("Sets the value of an Animation Clip")]
    
    [Image(typeof(IconAnimationClip), ColorTheme.Type.Teal)]

    [Category("Animations/Set Animation")]
    
    [Parameter("To", "The location where to save the Animation Clip")]
    [Parameter("Animation Clip", "The Animation Clip reference to store")]
    

    [Keywords("Animation", "Clip", "Animator")]
    [Serializable]
    public class InstructionAnimatorSetAnimation : Instruction
    {
        [SerializeField] protected PropertySetAnimation m_To = SetAnimationNone.Create;
        [SerializeField] private PropertyGetAnimation m_AnimationClip = GetAnimationInstance.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_To} = {this.m_AnimationClip}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            AnimationClip animationClip = this.m_AnimationClip.Get(args);
            this.m_To.Set(animationClip, args);
            
            return DefaultResult;
        }
    }
}