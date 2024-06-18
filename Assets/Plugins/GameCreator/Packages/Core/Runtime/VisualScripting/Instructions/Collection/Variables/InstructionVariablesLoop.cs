using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Loop List")]
    [Description("Loops a Game Object List Variables and executes an Actions component for each value")]
    
    [Image(typeof(IconInstructions), ColorTheme.Type.Blue, typeof(OverlayListVariable))]
    
    [Category("Variables/Loop List")]
    
    [Parameter("List Variable", "Local List or Global List which elements are iterated")]
    [Parameter(
        "Actions", 
        "The Actions component executed for each element in the list. The Target argument of " +
        "any Instruction contains the object inspected"
    )]

    [Keywords("Iterate", "Cycle", "Every", "All", "Stack")]
    [Serializable]
    public class InstructionVariablesLoop : Instruction
    {
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeField]
        private PropertyGetGameObject m_Actions = GetGameObjectActions.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Loop {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            Args actionsArgs = new Args(args.Self, null);
            List<object> source = this.m_ListVariable.Get(args);

            Actions actions = this.m_Actions.Get<Actions>(args);
            if (actions == null) return;

            for (int i = 0; i < source.Count; ++i)
            {
                GameObject gameObject = source[i] as GameObject;
                if (gameObject != null)
                {
                    actionsArgs.ChangeTarget(gameObject);
                }
                
                await actions.Run(actionsArgs);
            }
        }
    }
}