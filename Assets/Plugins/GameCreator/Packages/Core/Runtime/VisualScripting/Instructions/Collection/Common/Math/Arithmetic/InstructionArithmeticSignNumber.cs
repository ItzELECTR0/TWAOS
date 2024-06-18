using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Sign of Number")]
    [Description("Sets a value equal to -1 if the input number is negative. 1 otherwise")]

    [Category("Math/Arithmetic/Sign of Number")]

    [Parameter("Set", "Where the value is stored")]
    [Parameter("Number", "The input value")]

    [Keywords("Change", "Float", "Integer", "Variable", "Positive", "Negative")]
    [Image(typeof(IconContrast), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticSignNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Number = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = Sign({this.m_Number})";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_Number.Get(args);
            this.m_Set.Set(value >= 0 ? 1 : -1, args);

            return DefaultResult;
        }
    }
}