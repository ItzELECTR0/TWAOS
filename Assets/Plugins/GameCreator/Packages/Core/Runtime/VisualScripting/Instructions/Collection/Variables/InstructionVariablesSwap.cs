using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Swap List")]
    [Description("Swaps two positions of a list")]
    
    [Image(typeof(IconRepeat), ColorTheme.Type.Teal)]
    
    [Category("Variables/Swap List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are swapped")]

    [Keywords("Order", "Change", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesSwap : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeReference] private TListGetPick m_Element1 = new GetPickFirst();
        [SerializeReference] private TListGetPick m_Element2 = new GetPickLast();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Swap {this.m_ListVariable} {this.m_Element1} with {this.m_Element2}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);

            int index1 = this.m_Element1.GetIndex(elements.Count, args);
            int index2 = this.m_Element2.GetIndex(elements.Count, args);

            object value1 = elements[index1];
            object value2 = elements[index2];

            elements[index1] = value2;
            elements[index2] = value1;

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }
    }
}