using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Run Conditions")]
    [Description("Executes a Conditions component object")]

    [Category("Visual Scripting/Run Conditions")]

    [Parameter(
        "Conditions",
        "The Conditions object that is executed"
    )]

    [Parameter(
        "Wait Until Complete",
        "If true this instruction waits until the Conditions object finishes running"
    )]
    
    [Keywords("Execute", "Call", "Check", "Evaluate")]
    [Image(typeof(IconConditions), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionLogicRunConditions : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Conditions = GetGameObjectConditions.Create();
        
        [SerializeField] private bool m_WaitToFinish = true;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Run {0} {1}", 
            this.m_Conditions,
            this.m_WaitToFinish ? "and wait" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            Conditions conditions = this.m_Conditions.Get<Conditions>(args);
            if (conditions == null) return;

            if (this.m_WaitToFinish) await conditions.Run(args);
            else _ = conditions.Run(args);
        }
    }
}