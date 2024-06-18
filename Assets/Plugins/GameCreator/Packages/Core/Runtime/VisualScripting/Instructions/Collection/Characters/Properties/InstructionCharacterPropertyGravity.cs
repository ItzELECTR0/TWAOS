using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Gravity")]
    [Description("Changes the Character's gravity over time")]

    [Category("Characters/Properties/Change Gravity")]
    
    [Parameter("Mode", "Whether the upwards, downwards or both Gravity values are changed")]
    [Parameter("Gravity", "The target Gravity value for the Character")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Space")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyGravity : TInstructionCharacterProperty
    {
        private enum Mode
        {
            Both,
            GravityUpwards,
            GravityDownwards
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Mode m_Mode = Mode.Both;
        [SerializeField] private ChangeDecimal m_Gravity = new ChangeDecimal(-9.81f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "{0}Gravity {1} {2}",
            this.m_Mode switch
            {
                Mode.Both => string.Empty,
                Mode.GravityUpwards => "Upwards ",
                Mode.GravityDownwards => "Downwards ",
                _ => throw new ArgumentOutOfRangeException()
            },
            this.m_Character,
            this.m_Gravity
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float gravity = this.m_Mode switch
            {
                Mode.Both => (character.Motion.GravityUpwards + character.Motion.GravityDownwards) / 2f,
                Mode.GravityUpwards => character.Motion.GravityUpwards,
                Mode.GravityDownwards => character.Motion.GravityDownwards,
                _ => throw new ArgumentOutOfRangeException()
            };

            float valueSource = Math.Min(0f, gravity);
            float valueTarget = (float) this.m_Gravity.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) =>
                {
                    float value = Mathf.Lerp(a, b, t);
                    switch (this.m_Mode)
                    {
                        case Mode.Both:
                            character.Motion.GravityUpwards = value;
                            character.Motion.GravityDownwards = value;
                            break;
                        
                        case Mode.GravityUpwards:
                            character.Motion.GravityUpwards = value;
                            break;
                        
                        case Mode.GravityDownwards:
                            character.Motion.GravityDownwards = value;
                            break;
                        
                        default: throw new ArgumentOutOfRangeException();
                    }
                },
                Tween.GetHash(typeof(Character), "property:gravity"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}