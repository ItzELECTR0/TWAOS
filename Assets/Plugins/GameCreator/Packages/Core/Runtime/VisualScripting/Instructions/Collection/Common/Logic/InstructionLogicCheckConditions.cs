using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Check Conditions")]
    [Description(
        "If any of the Conditions list is false it early exits and skips the execution of " +
        "the rest of the Instructions below"
    )]

    [Category("Visual Scripting/Check Conditions")]

    [Parameter(
        "Conditions",
        "List of Conditions that can evaluate to true or false"
    )]
    
    [Parameter("Mode", "Whether to check the Conditions as an AND or an OR set")]

    [Keywords("Execute", "Call", "Check", "Evaluate")]
    [Image(typeof(IconCondition), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionLogicCheckConditions : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private ConditionList m_Conditions = new ConditionList();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title
        {
            get
            {
                string text = this.m_Conditions.Length switch
                {
                    0 => "(none)",
                    1 => $"Check {this.m_Conditions.Get(0)?.Title ?? "(unknown)"}",
                    _ => $"Check {this.m_Conditions.Length} Conditions"
                };

                return $"{text}";
            }
        }

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            if (!this.m_Conditions.Check(args, CheckMode.And))
            {
                this.NextInstruction = int.MaxValue;
            }
            
            return DefaultResult;
        }
    }
}