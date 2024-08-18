using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Green")]
    [Category("Colors/Green")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Green)]
    [Description("Returns the color Green #00FF00")]

    [Serializable]
    public class GetColorColorsGreen : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.green;

        public GetColorColorsGreen() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsGreen()
        );

        public override string String => "Green";
        
        public override Color EditorValue => Color.green;
    }
}