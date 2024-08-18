using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Tangent")]
    [Description("Sets a value equal the Tangent of a number")]

    [Category("Math/Arithmetic/Tangent")]

    [Parameter("Set", "Where the value is stored")]
    [Parameter("Tangent", "The angle input in radians")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconCurveCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticTangentNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Tangent = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = Tan({this.m_Tangent})";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_Tangent.Get(args);
            this.m_Set.Set(Math.Tan(value), args);

            return DefaultResult;
        }
    }
}