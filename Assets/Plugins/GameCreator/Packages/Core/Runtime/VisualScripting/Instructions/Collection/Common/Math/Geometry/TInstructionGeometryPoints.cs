using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Point 1", "The first operand of the geometric operation that represents a point in space")]
    [Parameter("Point 2", "The second operand of the geometric operation that represents a point in space")]

    [Keywords("Position", "Location", "Variable")]
    [Serializable]
    public abstract class TInstructionGeometryPoints : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetPosition m_Point1 = new PropertyGetPosition();
        
        [SerializeField]
        private PropertyGetPosition m_Point2 = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Operator { get; }
        
        public override string Title => string.Format(
            "Set {0} = {1} {2} {3}", 
            this.m_Set,
            this.m_Point1,
            this.Operator,
            this.m_Point2
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = this.Operate(
                this.m_Point1.Get(args),
                this.m_Point2.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract Vector3 Operate(Vector3 value1, Vector3 value2);
    }
}