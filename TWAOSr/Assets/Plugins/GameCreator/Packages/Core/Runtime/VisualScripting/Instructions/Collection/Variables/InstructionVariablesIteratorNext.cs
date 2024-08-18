using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Iterator Next")]
    [Description("Increases in one unit the value used as an iterator for a List Variable")]

    [Category("Variables/Iterator Next")]
    [Parameter("Index", "The numeric value used as an index")]
    [Parameter("List Variables", "The List Variable targeted")]
    [Parameter("Mode", "Whether the index loops back to the first index or is clamped")]

    [Keywords("Iterate", "Index", "For", "Loop", "Access")]
    [Image(typeof(IconListIndex), ColorTheme.Type.Teal, typeof(OverlayArrowRight))]

    [Serializable]
    public class InstructionVariablesIteratorNext : Instruction
    {
        private enum Mode
        {
            Circular,
            Clamp
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertySetNumber m_Index = SetNumberGlobalName.Create;
        
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeField]
        private Mode m_Mode = Mode.Circular;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Next {this.m_Index} Index for {this.m_ListVariable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> list = this.m_ListVariable.Get(args);
            if (list == null) return DefaultResult;

            int index = (int) this.m_Index.Get(args) + 1;

            switch (this.m_Mode)
            {
                case Mode.Circular:
                    index %= list.Count;
                    break;
                
                case Mode.Clamp:
                    index = Math.Clamp(index, 0, list.Count - 1);
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }

            this.m_Index.Set(index, args);
            return DefaultResult;
        }
    }
}