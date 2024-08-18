using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Wait Frames")]
    [Description("Waits a certain amount of frames")]

    [Category("Time/Wait Frames")]

    [Parameter(
        "Frames",
        "The amount of frames to wait"
    )]

    [Keywords("Wait", "Time", "Frames", "Yield")]
    [Image(typeof(IconTimer), ColorTheme.Type.Blue)]
    
    [Example(
        "This instruction is particularly useful in cases where you want to control the order " +
        "of execution of two Actions. For example, imagine there are two Triggers executing at " +
        "the same time, but you want to execute the instructions associated with one after " +
        "the execution of the other one. You can use the 'Wait Frames' instruction to defer " +
        "its execution 1 frame so the other one has had time to complete its own execution"
    )]
    
    [Serializable]
    public class InstructionCommonTimeFrame : Instruction
    {
        [SerializeField]
        private PropertyGetInteger m_Frames = new PropertyGetInteger(1);

        public override string Title =>
            $"Wait {this.m_Frames} {(this.m_Frames.ToString() == "1" ? "frame" : "frames")}";

        protected override async Task Run(Args args)
        {
            int frames = (int) this.m_Frames.Get(args);
            
            while (frames > 0)
            {
                frames -= 1;
                await this.NextFrame();
            }
        }
    }
}
