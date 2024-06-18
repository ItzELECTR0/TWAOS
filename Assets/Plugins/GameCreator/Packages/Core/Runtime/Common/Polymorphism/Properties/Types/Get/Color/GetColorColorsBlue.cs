using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Blue")]
    [Category("Colors/Blue")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Blue)]
    [Description("Returns the color Blue #0000FF")]

    [Serializable]
    public class GetColorColorsBlue : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.blue;

        public GetColorColorsBlue() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsBlue()
        );

        public override string String => "Blue";
        
        public override Color EditorValue => Color.blue;
    }
}