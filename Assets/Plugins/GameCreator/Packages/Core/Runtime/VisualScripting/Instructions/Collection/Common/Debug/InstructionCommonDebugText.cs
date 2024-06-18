using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Log Text")]
    [Description("Prints a message to the Unity Console")]

    [Category("Debug/Log Text")]

    [Parameter(
        "Message",
        "The text message to log"
    )]

    [Keywords("Debug", "Log", "Print", "Show", "Display", "Name", "Test", "Message", "String")]
    [Image(typeof(IconBug), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionCommonDebugText : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetString m_Message = new PropertyGetString("My message");

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Log: {this.m_Message}";

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InstructionCommonDebugText()
        { }

        public InstructionCommonDebugText(string text)
        {
            this.m_Message = new PropertyGetString(text);
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string value = this.m_Message.Get(args);
            Debug.Log(value);

            return DefaultResult;
        }
    }
}
