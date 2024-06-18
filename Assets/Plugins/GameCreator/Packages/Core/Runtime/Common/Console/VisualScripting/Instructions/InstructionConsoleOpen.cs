using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Runtime.Console
{
    [Version(0, 0, 1)]

    [Title("Console Open")]
    [Description("Opens the Runtime Console")]

    [Category("Debug/Console/Console Open")]

    [Keywords("Terminal", "Log", "Debug")]
    [Image(typeof(IconTerminal), ColorTheme.Type.Blue, typeof(OverlayPlus))]
    
    [Serializable]
    public class InstructionConsoleOpen : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => "Open Console";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Console.Open();
            return DefaultResult;
        }
    }
}
