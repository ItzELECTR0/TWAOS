using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("White")]
    [Category("Colors/White")]
    
    [Image(typeof(IconColor), ColorTheme.Type.White)]
    [Description("Returns the color White #FFFFFF")]

    [Serializable]
    public class GetColorColorsWhite : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.white;

        public GetColorColorsWhite() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsWhite()
        );

        public override string String => "White";
        
        public override Color EditorValue => Color.white;
    }
}