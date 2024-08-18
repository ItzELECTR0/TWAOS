using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Pause Editor")]
    [Description("Pauses the Editor. This has no effect on standalone applications")]

    [Category("Debug/Pause Editor")]

    [Keywords("Debug", "Break", "Pause", "Stop")]
    [Image(typeof(IconPause), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionCommonDebugPause : Instruction
    {
        public override string Title => "Pause Editor";

        protected override Task Run(Args args)
        {
            #if UNITY_EDITOR
            Debug.Break();
            #endif

            return DefaultResult;
        }
    }
}
