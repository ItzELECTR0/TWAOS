using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Stop Actions")]
    [Description("Stops an Actions component object that is being executed")]

    [Category("Visual Scripting/Stop Actions")]

    [Parameter(
        "Actions",
        "The Actions object that is stopped"
    )]

    [Keywords("Cancel", "Pause")]
    [Image(typeof(IconInstructions), ColorTheme.Type.Red, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionLogicStopActions : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Actions = GetGameObjectActions.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Stop {this.m_Actions}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Actions actions = this.m_Actions.Get<Actions>(args);
            if (actions == null) return DefaultResult;

            actions.Cancel();
            return DefaultResult;
        }
    }
}