using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Numbers")]
    [Description("Add two values together")]
    [Category("Math/Arithmetic/Add Numbers")]

    [Keywords("Sum", "Plus", "Float", "Integer", "Variable")]
    [Image(typeof(IconPlusCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticAddNumbers : TInstructionArithmetic
    {
        protected override string Operator => "+";
        
        protected override double Operate(double value1, double value2)
        {
            return value1 + value2;
        }
    }
}