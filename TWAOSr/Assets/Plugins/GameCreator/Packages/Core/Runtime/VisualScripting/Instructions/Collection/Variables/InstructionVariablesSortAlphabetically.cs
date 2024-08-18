using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Sort List Alphabetically")]
    [Description("Sorts the List Variable elements based on their alphabet distance")]
    
    [Image(typeof(IconSort), ColorTheme.Type.Teal)]
    
    [Category("Variables/Sort List Alphabetically")]
    
    [Parameter("List Variable", "Local List or Global List which elements are sorted")]
    [Parameter("Order", "Sort alphabetically ascending or descending")]
    [Parameter("Ignore Case", "Whether the string comparison should ignore upper/lower case")]

    [Keywords("Order", "Organize", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesSortAlphabetically : Instruction
    {
        private enum Order
        {
            Ascending,
            Descending
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeField] private Order m_Order = Order.Ascending;
        [SerializeField] private bool m_IgnoreCase = false;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Sort {this.m_ListVariable} {this.m_Order}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);
            
            elements.Sort(this.SortingMethod);

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }

        private int SortingMethod(object a, object b)
        {
            StringComparison comparison = this.m_IgnoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            return this.m_Order == Order.Ascending
                ? string.Compare(a.ToString(), b.ToString(), comparison)
                : string.Compare(b.ToString(), a.ToString(), comparison);
        }
    }
}