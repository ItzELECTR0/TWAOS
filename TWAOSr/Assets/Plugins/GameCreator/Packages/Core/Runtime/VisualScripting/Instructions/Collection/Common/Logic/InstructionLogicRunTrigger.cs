using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Run Trigger")]
    [Description("Executes a Trigger component object")]

    [Category("Visual Scripting/Run Trigger")]

    [Parameter(
        "Trigger",
        "The Trigger object that is executed"
    )]

    [Parameter(
        "Wait Until Complete",
        "If true this instruction waits until the Trigger object finishes running"
    )]
    
    [Keywords("Execute", "Call")]
    [Image(typeof(IconTriggers), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionLogicRunTrigger : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Trigger = GetGameObjectTrigger.Create();
        [SerializeField] private bool m_WaitToFinish = true;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Run {0} {1}", 
            this.m_Trigger,
            this.m_WaitToFinish ? "and wait" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            Trigger trigger = this.m_Trigger.Get<Trigger>(args);
            if (trigger == null) return;

            if (this.m_WaitToFinish) await trigger.Execute(args);
            else _ = trigger.Execute(args);
        }
    }
}