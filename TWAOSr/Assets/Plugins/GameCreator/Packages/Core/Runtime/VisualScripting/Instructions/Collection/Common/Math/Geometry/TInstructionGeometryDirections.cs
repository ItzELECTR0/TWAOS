using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Direction 1", "The first operand of the geometric operation that represents a direction")]
    [Parameter("Direction 2", "The second operand of the geometric operation that represents a direction")]

    [Keywords("Position", "Location", "Variable")]
    [Serializable]
    public abstract class TInstructionGeometryDirections : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetDirection m_Direction1 = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDirection m_Direction2 = new PropertyGetDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Operator { get; }
        
        public override string Title => string.Format(
            "Set {0} = {1} {2} {3}", 
            this.m_Set,
            this.m_Direction1,
            this.Operator,
            this.m_Direction2
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = this.Operate(
                this.m_Direction1.Get(args),
                this.m_Direction2.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract Vector3 Operate(Vector3 value1, Vector3 value2);
    }
}