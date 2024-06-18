namespace GameCreator.Runtime.Common.Mathematics
{
    internal class SymbolBinary : ISymbol
    {
        private readonly ISymbol m_LHS;
        private readonly ISymbol m_RHS;
        
        private readonly Parser.BinaryOperation m_Operation;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SymbolBinary(ISymbol lhs, ISymbol rhs, Parser.BinaryOperation operation)
        {
            this.m_LHS = lhs;
            this.m_RHS = rhs;
            
            this.m_Operation = operation;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Evaluate()
        {
            float valueA = this.m_LHS.Evaluate();
            float valueB = this.m_RHS.Evaluate();

            return this.m_Operation(valueA, valueB);
        }
    }
}