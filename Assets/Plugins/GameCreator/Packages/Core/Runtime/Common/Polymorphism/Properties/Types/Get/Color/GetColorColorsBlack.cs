using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Black")]
    [Category("Colors/Black")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Black)]
    [Description("Returns the color Black #000000")]

    [Serializable]
    public class GetColorColorsBlack : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.black;

        public GetColorColorsBlack() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsBlack()
        );

        public override string String => "Black";
        
        public override Color EditorValue => Color.black;
    }
}