using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Lerp Color")]
    [Description("Linearly interpolates between to colors over time")]

    [Category("Math/Shading/Lerp Color")]
    
    [Parameter("Color 1", "The starting Color value")]
    [Parameter("Color 2", "The targeted Color value")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the transition over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished or not")]

    [Keywords("Change", "Value", "Transition")]
    [Image(typeof(IconColor), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionShadingLerpColor : TInstructionShading
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetColor m_Color1 = GetColorColorsWhite.Create;
        [SerializeField] private PropertyGetColor m_Color2 = GetColorColorsBlue.Create;

        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Lerp from {this.m_Color1} to {this.m_Color2}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Color color1 = this.m_Color1.Get(args);
            Color color2 = this.m_Color2.Get(args);

            ITweenInput tween = new TweenInput<Color>(
                color1,
                color2,
                this.m_Transition.Duration,
                (a, b, t) => this.m_Set.Set(Color.Lerp(a, b, t), args),
                Tween.GetHash(typeof(GameObject), "color"),
                m_Transition.EasingType,
                m_Transition.Time
            );

            Tween.To(args.Self, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}