using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Clear List")]
    [Description("Removes all elements of a given Local or Global List Variables")]
    
    [Image(typeof(IconClear), ColorTheme.Type.Teal)]
    
    [Category("Variables/Clear List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are removed")]

    [Keywords("Clean", "Remove", "Delete", "Destroy", "Size", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesClear : Instruction
    {
        [SerializeField]
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Clear {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            this.m_ListVariable.Clear(args);
            return DefaultResult;
        }
    }
}