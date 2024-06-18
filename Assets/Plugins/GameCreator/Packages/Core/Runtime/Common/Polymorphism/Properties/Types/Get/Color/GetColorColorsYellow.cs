using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Yellow")]
    [Category("Colors/Yellow")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Yellow)]
    [Description("Returns the color Yellow (not #FFFF00 but nicer to look at!)")]

    [Serializable]
    public class GetColorColorsYellow : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.yellow;

        public GetColorColorsYellow() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsYellow()
        );

        public override string String => "Yellow";
        
        public override Color EditorValue => Color.yellow;
    }
}