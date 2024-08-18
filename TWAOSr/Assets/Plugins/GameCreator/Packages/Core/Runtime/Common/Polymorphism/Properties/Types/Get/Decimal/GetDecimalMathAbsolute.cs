using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Absolute Number")]
    [Category("Math/Arithmetic/Absolute Number")]
    
    [Image(typeof(IconAbsolute), ColorTheme.Type.TextNormal)]
    [Description("The numeric value without its sign")]

    [Keywords("Float", "Decimal", "Double", "Sign", "Module", "Magnitude")]
    
    [Serializable]
    public class GetDecimalMathAbsolute : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number = new PropertyGetDecimal();

        public override double Get(Args args)
        {
            double number2 = this.m_Number.Get(args);
            return Math.Abs(number2);
        }

        public override string String => $"|{this.m_Number}|";
    }
}