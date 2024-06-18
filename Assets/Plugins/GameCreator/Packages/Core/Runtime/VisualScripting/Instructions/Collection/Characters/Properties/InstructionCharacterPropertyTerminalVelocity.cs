using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Terminal Velocity")]
    [Description("Changes the Character's maximum fall-speed over time. Useful for gliding")]

    [Category("Characters/Properties/Change Terminal Velocity")]
    
    [Parameter("Terminal Velocity", "The target Terminal Velocity value for the Character")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Fall", "Glide", "Parachute", "Height")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyTerminalVelocity : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_TerminalVelocity = new ChangeDecimal(-53f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Terminal Velocity {this.m_Character} {this.m_TerminalVelocity}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = Math.Min(0f, character.Driver.WorldMoveDirection.y);
            float valueTarget = (float) this.m_TerminalVelocity.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Motion.TerminalVelocity = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:terminal-velocity"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}