using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Absolute Number")]
    [Description("Sets a value without its sign")]

    [Category("Math/Arithmetic/Absolute Number")]

    [Parameter("Set", "Where the value is stored")]
    [Parameter("Number", "The input value")]

    [Keywords("Change", "Float", "Integer", "Variable", "Sign", "Positive", "Modulus", "Magnitude")]
    [Image(typeof(IconAbsolute), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticAbsoluteNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Number = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = |{this.m_Number}|";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_Number.Get(args);
            this.m_Set.Set(Math.Abs(value), args);

            return DefaultResult;
        }
    }
}