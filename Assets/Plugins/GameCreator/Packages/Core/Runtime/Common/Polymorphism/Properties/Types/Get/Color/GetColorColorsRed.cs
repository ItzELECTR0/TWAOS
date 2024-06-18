using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Red")]
    [Category("Colors/Red")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Red)]
    [Description("Returns the color Red #FF0000")]

    [Serializable]
    public class GetColorColorsRed : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.red;

        public GetColorColorsRed() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsRed()
        );

        public override string String => "Red";
        
        public override Color EditorValue => Color.red;
    }
}