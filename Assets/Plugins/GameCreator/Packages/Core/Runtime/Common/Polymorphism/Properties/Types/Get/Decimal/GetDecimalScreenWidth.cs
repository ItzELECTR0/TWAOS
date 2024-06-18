using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Screen Width")]
    [Category("Screen/Screen Width")]
    
    [Image(typeof(IconSquareSolid), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("The horizontal size of the screen in pixels")]

    [Keywords("Resolution", "Size")]
    [Serializable]
    public class GetDecimalScreenWidth : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Screen.width;
        public override double Get(GameObject gameObject) => Screen.width;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalScreenWidth());

        public override string String => "Screen Width";
        
        public override double EditorValue => Screen.width;
    }
}