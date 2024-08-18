using System;

namespace GameCreator.Runtime.Common
{
    [Title("Random Unit")]
    [Category("Random/Random Unit")]
    
    [Image(typeof(IconDice), ColorTheme.Type.TextNormal)]
    [Description("A random decimal number between zero and one (range is inclusive)")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalRandomUnit : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => UnityEngine.Random.value;

        public GetDecimalRandomUnit() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalRandomUnit());

        public override string String => "Random Unit";
    }
}