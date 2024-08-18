using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Stop Conditions")]
    [Description("Stops a Conditions component object that is being executed")]

    [Category("Visual Scripting/Stop Conditions")]

    [Parameter(
        "Conditions",
        "The Conditions object that is stopped"
    )]

    [Keywords("Cancel", "Pause")]
    [Image(typeof(IconConditions), ColorTheme.Type.Red, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionLogicStopConditions : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Conditions = GetGameObjectConditions.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Stop {this.m_Conditions}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Conditions conditions = this.m_Conditions.Get<Conditions>(args);
            if (conditions == null) return DefaultResult;

            conditions.Cancel();
            return DefaultResult;
        }
    }
}