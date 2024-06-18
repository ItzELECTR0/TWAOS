using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Smooth Time")]
    [Description("Changes the average blend time between locomotion animations")]

    [Category("Characters/Animation/Change Smooth Time")]
    
    [Parameter("Smooth Time", "The target Smooth Time value. Values usually range between 0 and 0.5")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Example(
        "The Smooth Time controls how fast a Character animation blends into another when " +
        "reacting to external factors. A value of 0 makes the Character react instantly " +
        "whereas a value of 0.5 takes half a second to completely blend in. A value between " +
        "0.2 and 0.4 usually provide the best results, though it depends on the look and feel " +
        "the creator wants to achieve."
    )]
    
    [Keywords("Fade", "Realistic", "Old", "School", "Reaction")]
    [Image(typeof(IconAnimator), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterSmoothTime : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private ChangeDecimal m_SmoothTime = new ChangeDecimal(0.25f);
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Change Smooth Time of {this.m_Character} {this.m_SmoothTime}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float valueSource = character.Animim.SmoothTime;
            float valueTarget = (float) this.m_SmoothTime.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => character.Animim.SmoothTime = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(IUnitAnimim), "smooth-time"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}