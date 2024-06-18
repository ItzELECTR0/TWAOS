using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Radius")]
    [Description("Changes the Character's radius over time")]

    [Category("Characters/Properties/Change Radius")]
    
    [Parameter("Radius", "The target Radius value for the Character")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Diameter", "Space", "Fat", "Thin")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyRadius : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_Radius = new ChangeDecimal(0.5f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Radius {this.m_Character} {this.m_Radius}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = character.Motion.Radius;
            float valueTarget = (float) this.m_Radius.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Motion.Radius = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:radius"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}