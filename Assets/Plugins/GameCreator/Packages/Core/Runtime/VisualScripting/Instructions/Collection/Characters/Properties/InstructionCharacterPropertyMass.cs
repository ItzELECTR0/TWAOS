using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Mass")]
    [Description("Changes the Character's mass over time")]

    [Category("Characters/Properties/Change Mass")]
    
    [Parameter("Mass", "The target Mass value for the Character")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Weight")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyMass : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_Mass = new ChangeDecimal(80f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Mass of {this.m_Character} {this.m_Mass}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = character.Motion.Mass;
            float valueTarget = (float) this.m_Mass.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Motion.Mass = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:mass"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}