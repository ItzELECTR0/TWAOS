using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Subtract Numbers")]
    [Description("Subtracts the second value from the first one")]
    [Category("Math/Arithmetic/Subtract Numbers")]

    [Keywords("Rest", "Negative", "Minus", "Float", "Integer", "Variable")]
    [Image(typeof(IconMinusCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticSubtractNumbers : TInstructionArithmetic
    {
        protected override string Operator => "-";
        
        protected override double Operate(double value1, double value2)
        {
            return value1 - value2;
        }
    }
}