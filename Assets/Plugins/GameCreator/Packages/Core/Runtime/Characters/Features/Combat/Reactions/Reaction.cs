using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class Reaction : ScriptableObject, IReaction
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private float m_TransitionIn = 0.1f;
        [SerializeField] private float m_TransitionOut = 0.25f;

        [SerializeField] private bool m_UseRootMotion = true;
        [SerializeField] private PropertyGetDecimal m_Speed = GetDecimalConstantOne.Create;

        [SerializeField] private ReactionList m_ReactionList = new ReactionList();

        [SerializeField] private RunInstructionsList m_OnEnter = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnExit = new RunInstructionsList();

        // PROPERTIES: ----------------------------------------------------------------------------

        public float TransitionIn => this.m_TransitionIn;
        public float TransitionOut => this.m_TransitionOut;
        
        public bool UseRootMotion => this.m_UseRootMotion;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ReactionItem CanRun(Character character, Args args, ReactionInput input)
        {
            return character != null 
                ? this.m_ReactionList.Get(args, input.Direction, input.Power) 
                : null;
        }

        public ReactionOutput Run(Character character, Args args, ReactionInput input)
        {
            ReactionItem reaction = character != null 
                ? this.m_ReactionList.Get(args, input.Direction, input.Power) 
                : null;

            return this.Run(character, args, input, reaction);
        }

        public ReactionOutput Run(Character character, Args args, ReactionInput input, ReactionItem reaction)
        {
            if (character == null) return new ReactionOutput();
            if (reaction == null) return new ReactionOutput();

            this.RotateCharacter(character, input.Direction, reaction.Rotation);
            
            AnimationClip animationClip = reaction.AnimationClip;
            AvatarMask avatarMask = reaction.AvatarMask;
            
            float cancelTime = reaction.CancelTime;
            float gravity = reaction.Gravity;
            float speed = (float) this.m_Speed.Get(args);

            if (animationClip == null)
            {
                _ = this.m_OnEnter.Run(args);
                return new ReactionOutput();
            }

            ReactionOutput output = new ReactionOutput(
                animationClip.length,
                speed,
                cancelTime, 
                gravity,
                this
            );
            
            ConfigGesture config = new ConfigGesture(
                0f, animationClip != null ? animationClip.length : 0f, speed,
                this.m_UseRootMotion, 
                this.m_TransitionIn,
                this.m_TransitionOut
            );
            
            Task gestureTask = character.Gestures.CrossFade(
                animationClip, avatarMask, 
                BlendMode.Blend, 
                config, 
                true
            );

            _ = OnRun(this, gestureTask, args);

            return output;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RotateCharacter(Character character, Vector3 direction, ReactionRotation mode)
        {
            Vector3 flatDirection = Vector3.Scale(direction, Vector3Plane.NormalUp);
            if (flatDirection.sqrMagnitude <= 0f) return;
            
            switch (mode)
            {
                case ReactionRotation.None: return;

                case ReactionRotation.FollowDirection:
                    direction = flatDirection.normalized;
                    break;
                
                case ReactionRotation.AgainstDirection:
                    direction = -flatDirection.normalized;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }

            Quaternion rotation = Quaternion.LookRotation(
                character.transform.TransformDirection(direction),
                Vector3.up
            );
            
            character.Driver.SetRotation(rotation);
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static async Task OnRun(Reaction reaction, Task task, Args args)
        {
            if (reaction == null) return;
            _ = reaction.m_OnEnter.Run(args);
            
            await task;
            if (ApplicationManager.IsExiting) return;
            
            if (reaction == null) return;
            _ = reaction.m_OnExit.Run(args);
        }
    }
}