using System;

namespace GameCreator.Runtime.Common.Mathematics
{
    internal class Parser
    {
        public delegate float BinaryOperation(float a, float b);
        public delegate float UnaryOperation(float a);

        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly Tokenizer m_Tokenizer;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private Parser(string expression)
        {
            this.m_Tokenizer = new Tokenizer(expression);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static float Evaluate(string expression)
        {
            Parser parser = new Parser(expression);
            ISymbol symbol = parser.ParseExpression();
            
            if (symbol == null) return 0;
            return symbol.Evaluate();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private ISymbol ParseExpression()
        {
            ISymbol symbol = this.ParseAddSubtract();
            if (this.m_Tokenizer.Type != Tokenizer.TokenType.EndOfExpression)
            {
                throw new Exception("Unexpected characters at end of expression");
            }

            return symbol;
        }

        private ISymbol ParseAddSubtract()
        {
            ISymbol lhs = ParseMultiplyDivide();
            while (true)
            {
                BinaryOperation op = this.m_Tokenizer.Type switch
                {
                    Tokenizer.TokenType.Add => (a, b) => a + b,
                    Tokenizer.TokenType.Subtract => (a, b) => a - b,
                    _ => null
                };

                if (op == null) return lhs;
                this.m_Tokenizer.NextToken();

                ISymbol rhs = ParseMultiplyDivide();
                lhs = new SymbolBinary(lhs, rhs, op);
            }
        }

        private ISymbol ParseMultiplyDivide()
        {
            ISymbol lhs = ParseUnary();
            while (true)
            {
                BinaryOperation op = this.m_Tokenizer.Type switch
                {
                    Tokenizer.TokenType.Multiply => (a, b) => a * b,
                    Tokenizer.TokenType.Divide => (a, b) => a / b,
                    _ => null
                };

                if (op == null) return lhs;

                this.m_Tokenizer.NextToken();
                ISymbol rhs = this.ParseUnary();
                lhs = new SymbolBinary(lhs, rhs, op);
            }
        }

        private ISymbol ParseUnary()
        {
            if (this.m_Tokenizer.Type == Tokenizer.TokenType.Add)
            {
                this.m_Tokenizer.NextToken();
                return ParseUnary();
            }

            if (this.m_Tokenizer.Type == Tokenizer.TokenType.Subtract)
            {
                this.m_Tokenizer.NextToken();
                ISymbol rhs = ParseUnary();
                return new SymbolUnary(rhs, a => -a);
            }

            return this.ParseLeaf();
        }

        private ISymbol ParseLeaf()
        {
            if (this.m_Tokenizer.Type == Tokenizer.TokenType.Number)
            {
                ISymbol symbol = new SymbolNumber(this.m_Tokenizer.Number);
                this.m_Tokenizer.NextToken();
                return symbol;
            }

            if (this.m_Tokenizer.Type == Tokenizer.TokenType.OpenParenthesis)
            {
                this.m_Tokenizer.NextToken();
                ISymbol symbol = this.ParseAddSubtract();

                if (this.m_Tokenizer.Type != Tokenizer.TokenType.CloseParenthesis)
                {
                    throw new Exception("Missing closing parenthesis");
                }

                this.m_Tokenizer.NextToken();
                return symbol;
            }

            throw new Exception($"Unexpected token: {this.m_Tokenizer.Type}");
        }
    }
}