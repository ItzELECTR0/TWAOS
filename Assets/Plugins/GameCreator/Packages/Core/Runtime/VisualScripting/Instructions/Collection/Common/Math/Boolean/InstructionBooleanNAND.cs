using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("NAND Bool")]
    [Description("Executes a NAND operation between to values and saves the result")]

    [Category("Math/Boolean/NAND Bool")]

    [Keywords("Not", "Negative", "Subtract", "Minus", "Variable")]
    [Keywords("Boolean")]
    
    [Image(typeof(IconNAND), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionBooleanNAND : TInstructionBoolean
    {
        protected override string Operator => "NAND";
        
        protected override bool Operate(bool value1, bool value2)
        {
            return !(value1 && value2);
        }
    }
}