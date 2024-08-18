using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Subtract Decimals")]
    [Category("Math/Arithmetic/Subtract Decimals")]
    
    [Image(typeof(IconMinusCircle), ColorTheme.Type.TextNormal)]
    [Description("The result of subtracting two numbers")]

    [Keywords("Float", "Decimal", "Double", "Rest", "Minus", "Decrease", "Decrement")]
    
    [Serializable]
    public class GetDecimalMathSubtract : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number1 = new PropertyGetDecimal();
        [SerializeField] protected PropertyGetDecimal m_Number2 = new PropertyGetDecimal();

        public override double Get(Args args)
        {
            double number1 = this.m_Number1.Get(args);
            double number2 = this.m_Number2.Get(args);

            return number1 - number2;
        }

        public override string String => $"({this.m_Number1} - {this.m_Number2})";
    }
}