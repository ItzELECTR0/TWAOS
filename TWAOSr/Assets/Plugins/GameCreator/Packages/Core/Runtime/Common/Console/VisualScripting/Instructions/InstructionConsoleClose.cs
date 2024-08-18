using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Runtime.Console
{
    [Version(0, 0, 1)]

    [Title("Console Close")]
    [Description("Closes the Runtime Console")]

    [Category("Debug/Console/Console Close")]

    [Keywords("Terminal", "Log", "Debug")]
    [Image(typeof(IconTerminal), ColorTheme.Type.Blue, typeof(OverlayMinus))]
    
    [Serializable]
    public class InstructionConsoleClose : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => "Close Console";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Console.Close();
            return DefaultResult;
        }
    }
}
