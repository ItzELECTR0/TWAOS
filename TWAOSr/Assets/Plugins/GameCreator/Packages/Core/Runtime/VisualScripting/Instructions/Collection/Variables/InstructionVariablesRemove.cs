using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Remove from List")]
    [Description("Deletes an elements from a given Local or Global List Variables")]
    
    [Image(typeof(IconTrashOutline), ColorTheme.Type.Teal)]
    
    [Category("Variables/Remove from List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are removed")]

    [Keywords("Delete", "Destroy", "Size", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesRemove : Instruction
    {
        [SerializeField]
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeReference]
        private TListGetPick m_Select = new GetPickFirst(); 
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Remove {this.m_ListVariable}[{this.m_Select}]";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            this.m_ListVariable.Remove(this.m_Select, args);
            return DefaultResult;
        }
    }
}