using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Reverse List")]
    [Description("Reorders the elements of a list so the first ones become the last ones")]
    
    [Image(typeof(IconReverse), ColorTheme.Type.Teal)]
    
    [Category("Variables/Reverse List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are reversed")]

    [Keywords("Invert", "Order", "Sort", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesReverse : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Reverse {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);
            elements.Reverse();

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }
    }
}