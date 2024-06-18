using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Rotation")]
    [Description("Changes the rotation of a game object over time")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]

    [Category("Transforms/Change Rotation")]
    
    [Parameter("Rotation", "The desired rotation of the game object")]
    [Parameter("Space", "If the transformation occurs in local or world space")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the rotation over time")]
    [Parameter("Wait to Complete", "Whether to wait until the rotation is finished or not")]
    
    [Keywords("Rotate", "Angle", "Euler", "Tilt", "Pitch", "Yaw", "Roll")]
    [Serializable]
    public class InstructionTransformChangeRotation : TInstructionTransform
    {
        private enum SpaceMode
        {
            GlobalRotation = Space.World,
            LocalRotation = Space.Self
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private SpaceMode m_Space = SpaceMode.GlobalRotation;
        [SerializeField] private ChangeQuaternion m_Rotation = new ChangeQuaternion(
            Quaternion.Euler(0f, 180f, 0f)
        );
        
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Rotate {this.m_Transform} {this.m_Rotation}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return;

            Quaternion valueSource = this.m_Space switch
            {
                SpaceMode.GlobalRotation => gameObject.transform.rotation,
                SpaceMode.LocalRotation => gameObject.transform.localRotation,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Quaternion valueTarget = this.m_Rotation.Get(valueSource, args);
            
            ITweenInput tween = new TweenInput<Quaternion>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) =>
                {
                    switch (this.m_Space)
                    {
                        case SpaceMode.GlobalRotation:
                            gameObject.transform.rotation = Quaternion.LerpUnclamped(a, b, t);
                            break;
                        
                        case SpaceMode.LocalRotation:
                            gameObject.transform.localRotation = Quaternion.LerpUnclamped(a, b, t);
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                },
                Tween.GetHash(typeof(Transform), "rotation"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );

            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}