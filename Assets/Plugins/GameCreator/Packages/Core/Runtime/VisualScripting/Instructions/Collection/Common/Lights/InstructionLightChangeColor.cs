using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconLight), ColorTheme.Type.Yellow)]
    [Title("Light Color")]
    [Description("Smoothly changes the color of a Light component")]

    [Category("Lights/Light Color")]

    [Parameter("Color", "The color the Light component starts emitting")]

    [Keywords("Colour", "Hue", "Mood", "RGB", "Light")]
    [Serializable]
    public class InstructionLightChangeColor : TInstructionLight
    {
        [SerializeField] private ChangeColor m_Color = new ChangeColor();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Light Color of {this.m_Light} {this.m_Color}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Light.Get(args);
            if (gameObject == null) return;

            Light light = gameObject.Get<Light>();
            if (light == null) return;

            Color valueSource = light.color;
            Color valueTarget = this.m_Color.Get(valueSource, args);

            ITweenInput tween = new TweenInput<Color>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => light.color = Color.Lerp(a, b, t),
                Tween.GetHash(typeof(Light), "color"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}