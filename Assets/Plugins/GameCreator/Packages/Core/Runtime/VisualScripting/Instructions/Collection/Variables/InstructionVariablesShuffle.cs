using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Shuffle List")]
    [Description("Randomly shuffles the position of each element on a List Variable")]
    
    [Image(typeof(IconShuffle), ColorTheme.Type.Teal)]
    
    [Category("Variables/Shuffle List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are shuffled")]

    [Keywords("Randomize", "Sort", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesShuffle : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Shuffle {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);
            elements.Shuffle();

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }
    }
}