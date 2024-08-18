using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("-0.5")]
    [Category("Constant/Minus Point Five")]
    
    [Image(typeof(IconPointFive), ColorTheme.Type.TextNormal, typeof(OverlayMinus))]
    [Description("The unit -0.5 value")]
    
    [Serializable]
    public class GetDecimalConstantMinusPointFive : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => -0.5;
        public override double Get(GameObject gameObject) => -0.5;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantMinusPointFive());

        public override string String => "-0.5";
        
        public override double EditorValue => -0.5;
    }
}