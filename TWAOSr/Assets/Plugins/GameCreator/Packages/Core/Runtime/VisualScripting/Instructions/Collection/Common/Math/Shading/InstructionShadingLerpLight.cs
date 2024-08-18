using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Lerp Lightness")]
    [Description("Linearly interpolates between to the desired lightness value over time")]

    [Category("Math/Shading/Lerp Lightness")]
    
    [Parameter("Lightness", "The targeted Lightness value (between 0 and 1)")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the transition over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished or not")]

    [Keywords("Change", "Value", "Transition")]
    [Image(typeof(IconColor), ColorTheme.Type.Blue, typeof(OverlayZ))]
    
    [Serializable]
    public class InstructionShadingLerpLight : TInstructionShading
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetDecimal m_Lightness = GetDecimalDecimal.Create(1f);
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Lerp to Lightness: {this.m_Lightness}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Color color1 = this.m_Set.Get(args);
            
            Color.RGBToHSV(color1, out float hue, out float saturation, out float _);
            Color color2 = Color.HSVToRGB(hue, saturation, (float) this.m_Lightness.Get(args));

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