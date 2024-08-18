using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Value 1", "The first operand of the boolean operation")]
    [Parameter("Value 2", "The second operand of the boolean operation")]

    [Serializable]
    public abstract class TInstructionBoolean : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetBool m_Set = SetBoolGlobalName.Create;
        
        [SerializeField]
        private PropertyGetBool m_Value1 = new PropertyGetBool();
        
        [SerializeField]
        private PropertyGetBool m_Value2 = new PropertyGetBool();

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
            bool value = this.Operate(
                this.m_Value1.Get(args),
                this.m_Value2.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract bool Operate(bool value1, bool value2);
    }
}