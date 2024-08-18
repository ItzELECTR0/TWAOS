using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("0.5")]
    [Category("Constant/Point Five")]
    
    [Image(typeof(IconPointFive), ColorTheme.Type.TextNormal)]
    [Description("The unit 0.5 value")]
    
    [Serializable]
    public class GetDecimalConstantPointFive : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => 0.5;
        public override double Get(GameObject gameObject) => 0.5;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantPointFive());

        public override string String => "0.5";
        
        public override double EditorValue => 0.5;
    }
}