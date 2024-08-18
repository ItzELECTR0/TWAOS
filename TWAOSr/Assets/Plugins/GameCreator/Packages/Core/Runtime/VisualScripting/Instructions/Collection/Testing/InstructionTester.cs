using System;
using System.Text;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [HideInSelector]
    
    [Title("Tester")]
    [Category("Testing/Instruction Tester")]
    [Image(typeof(IconCheckSolid), ColorTheme.Type.Green)]

    [Description("Appends a character to a static Chain field. For internal testing use only")]
    
    [Parameter("Character", "A character that will be appended to InstructionTester.Chain")]
    
    [Example(@"
        Note that this Instruction is not accessible through the Inspector to avoid confusing new 
        users. To run the test suit environment, create a new `InstructionList` object and append
        as many `InstructionTester` instances as your test requires.
 
        ```csharp
        InstructionList instructions = new InstructionList(
            new InstructionTester('a'),
            new InstructionTester('b'),
            new InstructionTester('c')
        );

        InstructionTester.Clear();
        instructions.Run(null);

        Debug.Log(InstructionTester.Chain);
        // Prints: 'abc'
        ```
        This instruction is for internal testing only.
    ")]
    
    [Serializable]
    public class InstructionTester : Instruction
    {
        private static StringBuilder _Chain = new StringBuilder();
        
        public static string Chain => _Chain.ToString();
        public static void Clear() => _Chain.Clear();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private char m_Character = 'a';
        
        // TITLE: ---------------------------------------------------------------------------------

        public override string Title => $"Append '{this.m_Character}'";

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstructionTester() : base()
        { }

        public InstructionTester(char character) : this()
        {
            this.m_Character = character;
        }
        
        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            _Chain.Append(this.m_Character);
            return DefaultResult;
        }
    }
}