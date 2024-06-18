using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Dot Product")]
    [Description("Calculates the dot product between two directions and saves the result")]

    [Category("Math/Geometry/Dot Product")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Direction 1", "The first operand of the geometric operation that represents a direction")]
    [Parameter("Direction 2", "The second operand of the geometric operation that represents a direction")]
    
    [Keywords("Direction", "Parallel", "Perpendicular")]
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryDotProduct : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberNone.Create;
        
        [SerializeField]
        private PropertyGetDirection m_Direction1 = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDirection m_Direction2 = new PropertyGetDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title =>
            $"Set {this.m_Set} = {this.m_Direction1} · {this.m_Direction2}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            float value = Vector3.Dot(
                this.m_Direction1.Get(args),
                this.m_Direction2.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}