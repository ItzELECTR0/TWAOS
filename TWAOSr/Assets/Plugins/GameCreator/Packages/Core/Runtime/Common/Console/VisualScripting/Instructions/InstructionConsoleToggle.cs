using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Runtime.Console
{
    [Version(0, 0, 1)]

    [Title("Console Toggle")]
    [Description("Toggles the Runtime Console")]

    [Category("Debug/Console/Console Toggle")]

    [Keywords("Terminal", "Log", "Debug")]
    [Image(typeof(IconTerminal), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionConsoleToggle : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => "Toggle Open/Close Console";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Console.Toggle();
            return DefaultResult;
        }
    }
}
