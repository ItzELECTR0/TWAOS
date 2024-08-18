using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("1")]
    [Category("Constant/One")]
    
    [Image(typeof(IconOne), ColorTheme.Type.TextNormal)]
    [Description("The unit 1 value")]
    
    [Serializable]
    public class GetDecimalConstantOne : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => 1;
        public override double Get(GameObject gameObject) => 1;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantOne());

        public override string String => "1";

        public override double EditorValue => 1;
    }
}