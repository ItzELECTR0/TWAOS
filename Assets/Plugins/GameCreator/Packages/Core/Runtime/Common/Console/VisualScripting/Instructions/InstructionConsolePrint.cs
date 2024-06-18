using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    [Version(0, 0, 1)]

    [Title("Console Text")]
    [Description("Prints a message to the Runtime Console")]

    [Category("Debug/Console/Console Text")]

    [Parameter(
        "Message",
        "The text message to log"
    )]

    [Keywords("Debug", "Log", "Print", "Show", "Display", "Name", "Test", "Message", "String", "Terminal")]
    [Image(typeof(IconTerminal), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionConsolePrint : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetString m_Message = new PropertyGetString("My message");

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Log: {this.m_Message}";

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InstructionConsolePrint()
        { }

        public InstructionConsolePrint(string text)
        {
            this.m_Message = new PropertyGetString(text);
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string value = this.m_Message.Get(args);
            if (string.IsNullOrEmpty(value)) return DefaultResult;
            
            Console.Open();
            Console.Print(value);
            
            return DefaultResult;
        }
    }
}
