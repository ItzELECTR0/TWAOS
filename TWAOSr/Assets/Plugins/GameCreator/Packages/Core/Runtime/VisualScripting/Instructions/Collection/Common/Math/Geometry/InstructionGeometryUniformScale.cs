using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Uniform Scale")]
    [Description("Multiplies each component of a vector with a decimal")]

    [Category("Math/Geometry/Uniform Scale")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Vector", "The first operand of the geometric operation that represents a direction")]
    [Parameter("Value", "The second operand of the geometric operation that represents a decimal number")]
    
    [Keywords("Direction", "Homogeneous", "Multiply", "Product")]
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryUniformScale : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetDirection m_Direction = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDecimal m_Value = new PropertyGetDecimal(1f);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title =>
            $"Set {this.m_Set} = {this.m_Direction} * {this.m_Value}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = Vector3.Scale(
                this.m_Direction.Get(args),
                Vector3.one * (float) this.m_Value.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}