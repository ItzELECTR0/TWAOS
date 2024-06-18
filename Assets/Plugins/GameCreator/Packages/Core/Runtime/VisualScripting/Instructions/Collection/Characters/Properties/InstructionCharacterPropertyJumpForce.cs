using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Jump Force")]
    [Description("Changes the Character's jump force over time")]

    [Category("Characters/Properties/Change Jump Force")]
    
    [Parameter("Jump Force", "The target Jump Force value for the Character")]
    [Parameter("Duration", "How long it will take to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Hop", "Build", "Wind", "Fly")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyJumpForce : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_JumpForce = new ChangeDecimal(4f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Jump Force {this.m_Character} {this.m_JumpForce}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = character.Motion.JumpForce;
            float valueTarget = (float) this.m_JumpForce.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Motion.JumpForce = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:jump-force"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}