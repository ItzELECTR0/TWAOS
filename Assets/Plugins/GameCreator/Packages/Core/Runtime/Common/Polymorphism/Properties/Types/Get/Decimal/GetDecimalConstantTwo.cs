using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Two")]
    [Category("Constant/Two")]
    
    [Image(typeof(IconTwo), ColorTheme.Type.TextNormal)]
    [Description("The unit 2 value")]
    
    [Serializable]
    public class GetDecimalConstantTwo : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => 2;
        public override double Get(GameObject gameObject) => 2;

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalConstantTwo());

        public override string String => "2";

        public override double EditorValue => 2;
    }
}