using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("List is Empty")]
    [Description("Checks whether a List Variable is empty or not")]

    [Category("Variables/List is Empty")]
    
    [Parameter("List Variables", "The Local or Global List Variable to check")]

    [Keywords("Size", "Length", "Any", "Local", "Global", "Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Serializable]
    public class ConditionVariablesListEmpty : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private CollectorListVariable m_ListVariable = new CollectorListVariable();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_ListVariable} is Empty";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return this.m_ListVariable.GetCount(args) == 0;
        }
    }
}
