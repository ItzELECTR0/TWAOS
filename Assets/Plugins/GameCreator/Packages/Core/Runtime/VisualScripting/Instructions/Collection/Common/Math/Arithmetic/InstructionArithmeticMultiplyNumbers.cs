using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Multiply Numbers")]
    [Description("Multiplies two values together")]
    [Category("Math/Arithmetic/Multiply Numbers")]

    [Keywords("Product", "Float", "Integer", "Variable")]
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticMultiplyNumbers : TInstructionArithmetic
    {
        protected override string Operator => "*";
        
        protected override double Operate(double value1, double value2)
        {
            return value1 * value2;
        }
    }
}