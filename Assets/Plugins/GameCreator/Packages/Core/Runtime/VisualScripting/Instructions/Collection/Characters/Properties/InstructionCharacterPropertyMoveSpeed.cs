using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Movement Speed")]
    [Description("Changes the Character's maximum speed over time")]

    [Category("Characters/Properties/Change Movement Speed")]
    
    [Parameter("Speed", "The target movement Speed value for the Character")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Linear", "Walk", "Run", "Jog", "Sprint", "Velocity", "Throttle")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyMoveSpeed : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_Speed = new ChangeDecimal(4f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Move Speed {this.m_Character} {this.m_Speed}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = character.Motion.LinearSpeed;
            float valueTarget = (float) this.m_Speed.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Motion.LinearSpeed = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:linear-speed"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}