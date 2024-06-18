using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("One point Five")]
    [Category("Constant/One point Five")]
    
    [Image(typeof(IconOnePointFive), ColorTheme.Type.TextNormal)]
    [Description("The unit 1.5 value")]
    
    [Serializable]
    public class GetDecimalConstantOnePointFive : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => 1.5;
        public override double Get(GameObject gameObject) => 1.5;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantOnePointFive());

        public override string String => "1.5";
        
        public override double EditorValue => 1.5;
    }
}