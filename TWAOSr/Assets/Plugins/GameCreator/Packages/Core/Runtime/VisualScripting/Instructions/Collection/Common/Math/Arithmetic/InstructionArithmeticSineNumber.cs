using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Sine")]
    [Description("Sets a value equal the Sine of a number")]

    [Category("Math/Arithmetic/Sine")]

    [Parameter("Set", "Where the value is stored")]
    [Parameter("Sine", "The angle input in radians")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconCurveCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticSineNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Sine = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = Sin({this.m_Sine})";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_Sine.Get(args);
            this.m_Set.Set(Math.Sin(value), args);

            return DefaultResult;
        }
    }
}