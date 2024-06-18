using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Stop Trigger")]
    [Description("Stops a Trigger component object that is being executed")]

    [Category("Visual Scripting/Stop Trigger")]

    [Parameter(
        "Trigger",
        "The Trigger object that is stopped"
    )]

    [Keywords("Cancel", "Pause")]
    [Image(typeof(IconTriggers), ColorTheme.Type.Red, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionLogicStopTrigger : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Trigger = GetGameObjectTrigger.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Stop {this.m_Trigger}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Trigger trigger = this.m_Trigger.Get<Trigger>(args);
            if (trigger == null) return DefaultResult;

            trigger.Cancel();
            return DefaultResult;
        }
    }
}