using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("AND Bool")]
    [Description("Executes an AND operation between to values and saves the result")]

    [Category("Math/Boolean/AND Bool")]

    [Keywords("Subtract", "Minus", "Variable")]
    [Keywords("Boolean")]
    
    [Image(typeof(IconAND), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionBooleanAND : TInstructionBoolean
    {
        protected override string Operator => "AND";
        
        protected override bool Operate(bool value1, bool value2)
        {
            return value1 && value2;
        }
    }
}