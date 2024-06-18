using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Dash")]
    [Description("Moves the Character in the chosen direction for a brief period of time")]

    [Category("Characters/Navigation/Dash")]

    [Parameter("Direction", "Vector oriented towards the desired direction")]
    [Parameter("Velocity", "Velocity the Character moves throughout the whole movement")]
    [Parameter("Duration", "Defines the duration it takes to move forward at a constant velocity")]
    [Parameter("Wait to Finish", "If true this Instruction waits until the dash is completed")]
    [Parameter("Mode", "Whether to use Cardinal Animations (4 clips for each direction) or a single one")]
    [Parameter("Animation Speed", "Determines the speed coefficient applied to the animation played")]
    [Parameter("Transition In", "The time it takes to blend into the animation")]
    [Parameter("Transition Out", "The time it takes to blend out of the animation")]

    [Example(
        "The Transition Out parameter is also used to determine the movement blend between the " +
        "dash and the character's intended movement. Higher values will make characters take " +
        "longer to regain control after dashing"
    )]
    
    [Keywords("Leap", "Blink", "Roll", "Flash")]
    [Image(typeof(IconCharacterDash), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterNavigationDash : TInstructionCharacterNavigation
    {
        private const int DIRECTION_KEY = 5;
        
        [Serializable]
        public struct DashAnimation
        {
            public enum AnimationMode
            {
                CardinalAnimation,
                SingleAnimation
            }
            
            // EXPOSED MEMBERS: -------------------------------------------------------------------
            
            [SerializeField] private AnimationMode m_Mode;
            
            [SerializeField] private AnimationClip m_AnimationForward;
            [SerializeField] private AnimationClip m_AnimationBackward;
            [SerializeField] private AnimationClip m_AnimationRight;
            [SerializeField] private AnimationClip m_AnimationLeft;
            
            [SerializeField] private AnimationClip m_Animation;
            
            // PROPERTIES: ------------------------------------------------------------------------

            public AnimationMode Mode => this.m_Mode;

            public AnimationClip GetClip(float angle)
            {
                return this.m_Mode switch
                {
                    AnimationMode.CardinalAnimation => angle switch
                    {
                        <= 45f and >= -45f => this.m_AnimationForward,
                        < 135f and > 45f => this.m_AnimationLeft,
                        > -135f and < -45f => this.m_AnimationRight,
                        _ => this.m_AnimationBackward
                    },
                    AnimationMode.SingleAnimation => this.m_Animation,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionCharactersMoving.Create;
        [SerializeField] private PropertyGetDecimal m_Velocity = new PropertyGetDecimal(20f);
        [SerializeField] private PropertyGetDecimal m_Duration = new PropertyGetDecimal(0.25f);

        [SerializeField] [Range(0f, 1f)] private float m_Gravity = 1f;
        [SerializeField] private bool m_WaitToFinish = true;
        [SerializeField] private DashAnimation m_DashAnimation;
        
        [SerializeField] private float m_AnimationSpeed = 1f;
        [SerializeField] private float m_TransitionIn = 0.1f;
        [SerializeField] private float m_TransitionOut = 0.2f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Dash {this.m_Character} towards {this.m_Direction}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;
            if (character.Busy.AreLegsBusy) return;

            Vector3 direction = this.m_Direction.Get(args);
            if (direction == Vector3.zero) direction = character.transform.forward;
            
            float velocity = (float) this.m_Velocity.Get(args);
            float duration = (float) this.m_Duration.Get(args);
            
            if (!character.Dash.CanDash()) return;
            Task task = character.Dash.Execute(
                direction, velocity, this.m_Gravity, 
                duration, this.m_TransitionOut
            );

            character.Busy.MakeLegsBusy();
            float angle = Vector3.SignedAngle(
                direction, 
                character.transform.forward, 
                Vector3.up
            );

            AnimationClip animationClip = this.m_DashAnimation.GetClip(angle);

            if (animationClip != null)
            {
                ConfigGesture config = new ConfigGesture(
                    0f, animationClip.length, this.m_AnimationSpeed, false,
                    this.m_TransitionIn, this.m_TransitionOut
                );
                
                _ = character.Gestures.CrossFade(animationClip, null, BlendMode.Blend, config, true);
                
                if (this.m_DashAnimation.Mode == DashAnimation.AnimationMode.SingleAnimation)
                {
                    character.Kernel.Facing.SetLayerDirection(
                        DIRECTION_KEY, 
                        direction,
                        Math.Max(animationClip.length - this.m_TransitionOut, 0f)
                    );
                }
            }

            if (this.m_WaitToFinish) await task;
        }
    }
}