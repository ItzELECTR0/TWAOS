namespace GameCreator.Runtime.Common.Mathematics
{
    internal class SymbolNumber : ISymbol
    {
        private readonly float m_Number;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SymbolNumber(float number)
        {
            this.m_Number = number;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Evaluate()
        {
            return this.m_Number;
        }
    }
}