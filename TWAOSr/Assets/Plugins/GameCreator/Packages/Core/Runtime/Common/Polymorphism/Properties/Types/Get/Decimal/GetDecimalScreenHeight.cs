using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Screen Height")]
    [Category("Screen/Screen Height")]
    
    [Image(typeof(IconSquareSolid), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    [Description("The vertical size of the screen in pixels")]

    [Keywords("Resolution", "Size")]
    [Serializable]
    public class GetDecimalScreenHeight : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Screen.height;
        public override double Get(GameObject gameObject) => Screen.height;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalScreenHeight());

        public override string String => "Screen Height";
        
        public override double EditorValue => Screen.height;
    }
}