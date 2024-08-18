using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Opposite of Color")]
    [Category("Math/Opposite of Color")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    [Description("Returns the opposite of the color value")]

    [Serializable]
    public class GetColorOpposite : PropertyTypeGetColor
    {
        [SerializeField]
        protected PropertyGetColor m_Color = new PropertyGetColor();

        public override Color Get(Args args)
        {
            Color color = this.m_Color.Get(args);
            
            Color.RGBToHSV(color, out float hue, out float saturation, out float light);
            return Color.HSVToRGB((hue + 0.5f) % 1f, saturation, light);
        }

        public static PropertyGetColor Create(Color value) => new PropertyGetColor(
            new GetColorValue(value)
        );

        public override string String => $"Opposite of {this.m_Color}";
    }
}