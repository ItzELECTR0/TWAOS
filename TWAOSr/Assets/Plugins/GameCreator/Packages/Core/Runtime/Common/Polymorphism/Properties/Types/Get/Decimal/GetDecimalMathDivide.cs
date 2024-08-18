using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Divide Decimals")]
    [Category("Math/Arithmetic/Divide Decimals")]
    
    [Image(typeof(IconDivideCircle), ColorTheme.Type.TextNormal)]
    [Description("The result of dividing two numbers")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable]
    public class GetDecimalMathDivide : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number1 = new PropertyGetDecimal();
        [SerializeField] protected PropertyGetDecimal m_Number2 = new PropertyGetDecimal();

        public override double Get(Args args)
        {
            double number1 = this.m_Number1.Get(args);
            double number2 = this.m_Number2.Get(args);

            return number1 / number2;
        }

        public override string String => $"({this.m_Number1} / {this.m_Number2})";
    }
}