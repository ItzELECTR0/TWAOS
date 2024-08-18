using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("-2")]
    [Category("Constant/Minus Two")]
    
    [Image(typeof(IconTwo), ColorTheme.Type.TextNormal, typeof(OverlayMinus))]
    [Description("The unit -2 value")]
    
    [Serializable]
    public class GetDecimalConstantMinusTwo : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => -2;
        public override double Get(GameObject gameObject) => -2;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalConstantMinusTwo()
        );

        public override string String => "-2";

        public override double EditorValue => -2;
    }
}