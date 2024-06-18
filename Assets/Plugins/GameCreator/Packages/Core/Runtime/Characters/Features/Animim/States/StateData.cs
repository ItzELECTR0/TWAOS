using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public struct StateData
    {
        public enum StateType
        {
            AnimationClip,
            RuntimeController,
            State
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private StateType m_StateType;

        [SerializeField] private PropertyGetAnimation m_AnimationClip;
        [SerializeField] private RuntimeAnimatorController m_RuntimeController;
        [SerializeField] private State m_State;

        [SerializeField] private AvatarMask m_AvatarMask;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public StateType Type => this.m_StateType;
        
        public RuntimeAnimatorController RuntimeController => this.m_RuntimeController;
        public State State => this.m_State;

        public AvatarMask AvatarMask => this.m_AvatarMask;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public StateData(StateType stateType)
        {
            this.m_StateType = stateType;
            m_AnimationClip = GetAnimationInstance.Create;
            m_RuntimeController = null;
            m_State = null;
            m_AvatarMask = null;
        }

        public StateData(AnimationClip animationClip, AvatarMask avatarMask) 
            : this(StateType.AnimationClip)
        {
            this.m_AnimationClip = new PropertyGetAnimation(
                new GetAnimationInstance(animationClip)
            );
            
            this.m_AvatarMask = avatarMask;
        }
        
        public StateData(RuntimeAnimatorController runtimeController, AvatarMask avatarMask) 
            : this(StateType.RuntimeController)
        {
            this.m_RuntimeController = runtimeController;
            this.m_AvatarMask = avatarMask;
        }
        
        public StateData(StateOverrideAnimator state) : this(StateType.State)
        {
            this.m_State = state;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public AnimationClip GetAnimationClip(Args args)
        {
            return this.m_AnimationClip.Get(args);
        }

        public bool IsValid(Character character)
        {
            return this.m_StateType switch
            {
                StateType.AnimationClip => this.m_AnimationClip.Get(character.Args) != null,
                StateType.RuntimeController => this.m_RuntimeController != null,
                StateType.State => this.m_State != null,
                _ => false
            };
        }
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string ToString()
        {
            switch (this.m_StateType)
            {
                case StateType.AnimationClip:
                    return this.m_AnimationClip.EditorValue != null
                        ? this.m_AnimationClip.EditorValue.name
                        : "(none)";
                
                case StateType.RuntimeController:
                    return this.m_RuntimeController != null
                        ? this.m_RuntimeController.name
                        : "(none)";
                
                case StateType.State:
                    return this.m_State != null
                        ? this.m_State.name
                        : "(none)";
                
                default: return string.Empty;
            }
        }
    }
}