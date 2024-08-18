using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Cyan")]
    [Category("Colors/Cyan")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Teal)]
    [Description("Returns the color Cyan #00FFFF")]

    [Serializable]
    public class GetColorColorsCyan : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.cyan;

        public GetColorColorsCyan() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsCyan()
        );

        public override string String => "Cyan";
        
        public override Color EditorValue => Color.cyan;
    }
}