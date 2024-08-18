using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Value 1", "The first operand of the arithmetic operation")]
    [Parameter("Value 2", "The second operand of the arithmetic operation")]

    [Serializable]
    public abstract class TInstructionArithmetic : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Value1 = new PropertyGetDecimal();
        
        [SerializeField]
        private PropertyGetDecimal m_Value2 = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Operator { get; }
        
        public override string Title => string.Format(
            "Set {0} = {1} {2} {3}", 
            this.m_Set,
            this.m_Value1,
            this.Operator,
            this.m_Value2
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value1 = this.m_Value1.Get(args);
            double value2 = this.m_Value2.Get(args);
            
            double value = this.Operate(value1,  value2);
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract double Operate(double value1, double value2);
    }
}