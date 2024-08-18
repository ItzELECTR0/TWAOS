using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Number")]
    [Description("Sets a value equal to another value")]

    [Category("Math/Arithmetic/Set Number")]

    [Parameter("Set", "Where the value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconArrowCircleDown), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticSetNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_From = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            double value = this.m_From.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}