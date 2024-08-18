using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("-1")]
    [Category("Constant/Minus One")]
    
    [Image(typeof(IconOne), ColorTheme.Type.TextNormal, typeof(OverlayMinus))]
    [Description("The unit -1 value")]
    
    [Serializable]
    public class GetDecimalConstantMinusOne : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => -1;
        public override double Get(GameObject gameObject) => -1;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantMinusOne());

        public override string String => "-1";

        public override double EditorValue => -1;
    }
}