namespace GameCreator.Runtime.Common.Mathematics
{
    internal class SymbolUnary : ISymbol
    {
        private readonly ISymbol m_RHS;
        private readonly Parser.UnaryOperation m_Operation;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SymbolUnary(ISymbol rhs, Parser.UnaryOperation operation)
        {
            this.m_RHS = rhs;
            this.m_Operation = operation;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Evaluate()
        {
            float value = this.m_RHS.Evaluate();
            return this.m_Operation(value);
        }
    }
}