using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Magenta")]
    [Category("Colors/Magenta")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Pink)]
    [Description("Returns the color Magenta #FF00FF")]

    [Serializable]
    public class GetColorColorsMagenta : PropertyTypeGetColor
    {
        public override Color Get(Args args) => Color.magenta;

        public GetColorColorsMagenta() : base()
        { }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorColorsMagenta()
        );

        public override string String => "Magenta";
        
        public override Color EditorValue => Color.magenta;
    }
}