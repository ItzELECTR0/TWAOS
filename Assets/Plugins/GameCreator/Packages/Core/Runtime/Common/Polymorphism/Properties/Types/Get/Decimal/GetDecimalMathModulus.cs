using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Modulus Decimals")]
    [Category("Math/Arithmetic/Modulus Decimals")]
    
    [Image(typeof(IconDivideCircle), ColorTheme.Type.TextNormal, typeof(OverlayArrowRight))]
    [Description("The modulus operation, which is what's left of the division between two numbers")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable]
    public class GetDecimalMathModulus : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number1 = new PropertyGetDecimal();
        [SerializeField] protected PropertyGetDecimal m_Number2 = new PropertyGetDecimal();

        public override double Get(Args args)
        {
            double number1 = this.m_Number1.Get(args);
            double number2 = this.m_Number2.Get(args);

            return number1 % number2;
        }

        public override string String => $"({this.m_Number1} % {this.m_Number2})";
    }
}