using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Move List")]
    [Description("Move a position from a list to another position")]
    
    [Image(typeof(IconMove), ColorTheme.Type.Teal)]
    
    [Category("Variables/Move List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are moved")]

    [Keywords("Order", "Change", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesMove : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();
        [SerializeReference] private TListGetPick m_From = new GetPickLast();
        [SerializeReference] private TListGetPick m_To = new GetPickLast();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Move {this.m_ListVariable} {this.m_From} to {this.m_To}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);

            int index1 = this.m_From.GetIndex(elements.Count, args);
            int index2 = this.m_To.GetIndex(elements.Count, args);

            object value = elements[index1];

            elements.RemoveAt(index1);
            elements.Insert(index2, value);

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }
    }
}