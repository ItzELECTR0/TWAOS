namespace GameCreator.Runtime.VisualScripting
{
    public class InstructionResult
	{
        public static readonly InstructionResult Default = JumpTo(1);
		public static readonly InstructionResult Stop = StopPropagation();

        // PROPERTIES: ----------------------------------------------------------------------------

		public int NextInstruction { get; private set; }
		public bool DontContinue { get; private set; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

		public static InstructionResult JumpTo(int nextInstruction)
        {
			return new InstructionResult
			{
				NextInstruction = nextInstruction,
				DontContinue = false
			};
        }

        private static InstructionResult StopPropagation()
        {
            return new InstructionResult
            {
                NextInstruction = 1,
                DontContinue = true
            };
        }
	}
}