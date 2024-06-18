using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Iterator Random")]
    [Description("Sets a random value between zero and the list count")]

    [Category("Variables/Iterator Random")]
    [Parameter("Index", "The numeric value used as an index")]
    [Parameter("List Variables", "The List Variable targeted")]

    [Keywords("Iterate", "Index", "For", "Loop", "Access")]
    [Image(typeof(IconListIndex), ColorTheme.Type.Teal, typeof(OverlayDice))]

    [Serializable]
    public class InstructionVariablesIteratorRandom : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertySetNumber m_Index = SetNumberGlobalName.Create;
        
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Random {this.m_Index} Index for {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> list = this.m_ListVariable.Get(args);
            if (list == null) return DefaultResult;

            int random = UnityEngine.Random.Range(0, list.Count);
            this.m_Index.Set(random, args);
            
            return DefaultResult;
        }
    }
}