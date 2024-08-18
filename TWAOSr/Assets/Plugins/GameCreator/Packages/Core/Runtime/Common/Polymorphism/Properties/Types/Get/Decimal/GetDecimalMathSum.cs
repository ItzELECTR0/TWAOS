using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Add Decimals")]
    [Category("Math/Arithmetic/Add Decimals")]
    
    [Image(typeof(IconPlusCircle), ColorTheme.Type.TextNormal)]
    [Description("The result of adding two numbers")]

    [Keywords("Float", "Decimal", "Double", "Add", "Increase", "Increment", "Plus")]
    
    [Serializable]
    public class GetDecimalMathSum : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number1 = new PropertyGetDecimal();
        [SerializeField] protected PropertyGetDecimal m_Number2 = new PropertyGetDecimal();

        public override double Get(Args args)
        {
            double number1 = this.m_Number1.Get(args);
            double number2 = this.m_Number2.Get(args);

            return number1 + number2;
        }

        public override string String => $"({this.m_Number1} + {this.m_Number2})";
    }
}