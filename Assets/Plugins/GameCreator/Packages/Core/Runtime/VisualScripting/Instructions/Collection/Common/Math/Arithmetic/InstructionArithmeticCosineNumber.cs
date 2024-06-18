using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Cosine")]
    [Description("Sets a value equal the Cosine of a number")]

    [Category("Math/Arithmetic/Cosine")]

    [Parameter("Set", "Where the value is stored")]
    [Parameter("Cosine", "The angle input in radians")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconCurveCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticCosineNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Cosine = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = Cos({this.m_Cosine})";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_Cosine.Get(args);
            this.m_Set.Set(Math.Cos(value), args);

            return DefaultResult;
        }
    }
}