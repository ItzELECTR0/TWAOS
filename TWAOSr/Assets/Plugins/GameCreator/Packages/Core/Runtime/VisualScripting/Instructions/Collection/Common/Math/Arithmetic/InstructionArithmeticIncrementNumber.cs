using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Increment Number")]
    [Description("Sets a value equal the sum of itself, plus another number")]

    [Category("Math/Arithmetic/Increment Number")]

    [Parameter("Set", "The value being incremented")]
    [Parameter("Value", "The value to add")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconPlusCircle), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionArithmeticIncrementNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Value = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Increment {this.m_Set} + {this.m_Value}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value1 = this.m_Set.Get(args);
            double value2 = this.m_Value.Get(args);

            this.m_Set.Set(value1 + value2, args);
            return DefaultResult;
        }
    }
}