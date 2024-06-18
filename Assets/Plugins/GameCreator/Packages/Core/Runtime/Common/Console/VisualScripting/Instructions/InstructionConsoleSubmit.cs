using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    [Version(0, 0, 1)]

    [Title("Console Command")]
    [Description("Submits a Command onto the Runtime Console")]

    [Category("Debug/Console/Console Command")]

    [Parameter(
        "Command",
        "The command message to submit"
    )]

    [Keywords("Debug", "Log", "Terminal", "Submit", "Send", "Execute", "Run")]
    [Image(typeof(IconTerminal), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionConsoleSubmit : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetString m_Command = new PropertyGetString("run name Actions");

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Submit: {this.m_Command}";

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InstructionConsoleSubmit()
        { }

        public InstructionConsoleSubmit(string text)
        {
            this.m_Command = new PropertyGetString(text);
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string text = this.m_Command.Get(args);
            if (string.IsNullOrEmpty(text)) return DefaultResult;
            
            Console.Open();

            Input input = new Input(text);
            Console.Submit(input);
            
            return DefaultResult;
        }
    }
}
