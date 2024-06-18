using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Black & White of Color")]
    [Category("Math/Black & White of Color")]
    
    [Image(typeof(IconColor), ColorTheme.Type.TextNormal)]
    [Description("Returns the black and white value of the color")]

    [Serializable]
    public class GetColorBlackAndWhite : PropertyTypeGetColor
    {
        [SerializeField]
        protected PropertyGetColor m_Color = new PropertyGetColor();

        public override Color Get(Args args)
        {
            Color color = this.m_Color.Get(args);
            
            Color.RGBToHSV(color, out float hue, out float _, out float light);
            return Color.HSVToRGB(hue, 0f, light);
        }

        public static PropertyGetColor Create(Color value) => new PropertyGetColor(
            new GetColorValue(value)
        );

        public override string String => $"Black & White of {this.m_Color}";
    }
}