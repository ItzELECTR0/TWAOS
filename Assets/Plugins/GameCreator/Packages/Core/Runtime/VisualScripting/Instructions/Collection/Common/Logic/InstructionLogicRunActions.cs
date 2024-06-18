using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Run Actions")]
    [Description("Executes an Actions component object")]

    [Category("Visual Scripting/Run Actions")]

    [Parameter(
        "Actions",
        "The Actions object that is executed"
    )]

    [Parameter(
        "Wait Until Complete",
        "If true this instruction waits until the Actions object finishes running"
    )]
    
    [Keywords("Execute", "Call", "Instruction", "Action")]
    [Image(typeof(IconInstructions), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionLogicRunActions : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Actions = GetGameObjectActions.Create();
        [SerializeField] private bool m_WaitToFinish = true;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Run {0} {1}", 
            this.m_Actions,
            this.m_WaitToFinish ? "and wait" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            Actions actions = this.m_Actions.Get<Actions>(args);
            if (actions == null) return;
            
            if (this.m_WaitToFinish) await actions.Run(args);
            else _ = actions.Run(args);
        }
    }
}