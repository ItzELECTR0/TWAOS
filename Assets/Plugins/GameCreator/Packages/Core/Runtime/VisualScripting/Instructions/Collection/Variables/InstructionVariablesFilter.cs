using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Filter List")]
    [Description(
        "Checks Conditions against each element of a list and removes it if the Condition " +
        "is not true"
    )]
    
    [Image(typeof(IconFilter), ColorTheme.Type.Teal)]
    
    [Category("Variables/Filter List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are filtered")]
    [Parameter(
        "Filter", 
        "Checks a set of Conditions with each collected game object and removes the element " +
        "if the Condition is not true"
    )]

    [Example(
        "The Filter field runs the Conditions list for each element in a Local List Variables " +
        "or Global List Variables. It sets as the 'Target' value the currently examined game " +
        "object. For example, filtering by the tag name 'Enemy' can be done using the 'Tag' " +
        "Condition and comparing the field 'Target' with the string 'Enemy'. All game objects " +
        "that are not tagged as 'Enemy' are removed"
    )]
    
    [Keywords("Remove", "Pick", "Select", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesFilter : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();
        
        [SerializeField] private ConditionList m_Conditions = new ConditionList();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Filter {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Args filterArgs = new Args(args.Self, null);
            List<object> source = this.m_ListVariable.Get(args);
            List<GameObject> destination = new List<GameObject>();

            for (int i = 0; i < source.Count; ++i)
            {
                GameObject gameObject = source[i] as GameObject;
                if (gameObject == null) continue;
                
                filterArgs.ChangeTarget(gameObject);
                if (this.m_Conditions.Check(filterArgs, CheckMode.And))
                {
                    destination.Add(gameObject);
                }
            }

            this.m_ListVariable.Fill(destination.ToArray(), args);
            return DefaultResult;
        }
    }
}