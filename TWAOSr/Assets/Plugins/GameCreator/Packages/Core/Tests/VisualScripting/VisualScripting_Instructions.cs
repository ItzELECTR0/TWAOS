using GameCreator.Runtime.VisualScripting;
using NUnit.Framework;

namespace GameCreator.Tests.Core
{
    public class VisualScripting_Instructions
    {
        [Test]
        public void ForwardInstructions_Passes()
        {
            InstructionList instructions = new InstructionList(
                new InstructionTester('a'),
                new InstructionTester('b'),
                new InstructionTester('c')
            );

            InstructionTester.Clear();
            _ = instructions.Run(null);

            Assert.AreEqual("abc", InstructionTester.Chain);
        }

        [Test]
        public void ArbitraryIndexedInstructions_Passes()
        {
            InstructionList instructions = new InstructionList(
                new InstructionTester('a'),
                new InstructionTester('b'),
                new InstructionTester('c')
            );

            InstructionTester.Clear();
            _ = instructions.Run(null, 1);
            
            Assert.AreEqual("bc", InstructionTester.Chain);
        }
        
        [Test]
        public void NegativeIndexedInstructions_Passes()
        {
            InstructionList instructions = new InstructionList(
                new InstructionTester('a'),
                new InstructionTester('b'),
                new InstructionTester('c')
            );

            InstructionTester.Clear();
            _ = instructions.Run(null, -1);

            Assert.AreEqual("abc", InstructionTester.Chain);
        }
        
        [Test]
        public void OverflowIndexedInstructions_Passes()
        {
            InstructionList instructions = new InstructionList(
                new InstructionTester('a'),
                new InstructionTester('b'),
                new InstructionTester('c')
            );

            InstructionTester.Clear();
            _ = instructions.Run(null, 9999);

            Assert.AreEqual("", InstructionTester.Chain);
        }
    }
}
