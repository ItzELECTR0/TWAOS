using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Clamp Number")]
    [Description("Clamps a value between a range defined by two others (inclusive)")]
    [Category("Math/Arithmetic/Clamp Number")]

    [Keywords("Min", "Max", "Negative", "Minus", "Float", "Integer", "Variable")]
    [Image(typeof(IconContrast), ColorTheme.Type.Blue)]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Value", "The value that is clamped between two others")]
    [Parameter("Minimum", "The smallest possible value")]
    [Parameter("Maximum", "The largest possible value")]

    [Serializable]
    public class InstructionArithmeticClampNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Value = new PropertyGetDecimal();

        [SerializeField] private float m_Minimum = 0f;
        [SerializeField] private float m_Maximum = 1f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title =>
            $"Clamp {this.m_Set} = {this.m_Value} [{this.m_Minimum}, {this.m_Maximum}]";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = Math.Clamp(
                this.m_Value.Get(args),
                this.m_Minimum,
                this.m_Maximum
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}