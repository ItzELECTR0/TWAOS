using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Restart Instructions")]
    [Description("Stops executing the current list of Instructions and starts again from the top")]

    [Category("Visual Scripting/Restart Instructions")]

    [Keywords("Reset", "Call", "Again")]
    [Image(typeof(IconInstructions), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    
    [Serializable]
    public class InstructionLogicRestartInstructions : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Restart Instructions";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            this.NextInstruction = -this.Parent.RunningIndex;
            return DefaultResult;
        }
    }
}