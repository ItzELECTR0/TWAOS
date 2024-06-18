using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Modulus Numbers")]
    [Description("Calculates the modulus between the first and the second value")]
    [Category("Math/Arithmetic/Modulus Numbers")]

    [Keywords("Fraction", "Float", "Integer", "Variable", "Module")]
    [Image(typeof(IconDivideCircle), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionArithmeticModulusNumbers : TInstructionArithmetic
    {
        protected override string Operator => "%";
        
        protected override double Operate(double value1, double value2)
        {
            return value1 % value2;
        }
    }
}