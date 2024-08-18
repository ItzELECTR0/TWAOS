using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("0")]
    [Category("Constant/Zero")]
    
    [Image(typeof(IconZero), ColorTheme.Type.TextNormal)]
    [Description("The zero value")]
    
    [Serializable]
    public class GetDecimalConstantZero : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => 0;
        public override double Get(GameObject gameObject) => 0;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantZero());

        public override string String => "0";
        
        public override double EditorValue => 0;
    }
}