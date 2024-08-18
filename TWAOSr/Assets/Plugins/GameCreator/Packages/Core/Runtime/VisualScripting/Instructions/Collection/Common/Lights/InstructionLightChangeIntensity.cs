using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconLight), ColorTheme.Type.Yellow)]
    [Title("Light Intensity")]
    [Description("Smoothly changes the intensity of a Light component")]

    [Category("Lights/Light Intensity")]

    [Parameter("Intensity", "The intensity change that the Light component undergoes")]

    [Serializable]
    public class InstructionLightChangeIntensity : TInstructionLight
    {
        [SerializeField] private ChangeDecimal m_Intensity = new ChangeDecimal(5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Light Intensity of {this.m_Light} {this.m_Intensity}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Light.Get(args);
            if (gameObject == null) return;

            Light light = gameObject.Get<Light>();
            if (light == null) return;

            float valueSource = light.intensity;
            float valueTarget = (float) this.m_Intensity.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => light.intensity = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Light), "intensity"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}