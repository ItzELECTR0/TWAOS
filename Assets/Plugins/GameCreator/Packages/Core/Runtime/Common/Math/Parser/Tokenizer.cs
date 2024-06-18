using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GameCreator.Runtime.Common.Mathematics
{
    public class Tokenizer
    {
        public enum TokenType
        {
            EndOfExpression,
            Add,
            Subtract,
            Multiply,
            Divide,
            OpenParenthesis,
            CloseParenthesis,
            Number,
        }
        
        // CONSTANTS: -----------------------------------------------------------------------------

        private static readonly StringBuilder StringBuilder = new StringBuilder();
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        
        private const char CHAR_EOE = '\0';
        private const char CHAR_ADD = '+';
        private const char CHAR_SUBTRACT = '-';
        private const char CHAR_MULTIPLY = '*';
        private const char CHAR_DIVIDE = '/';
        private const char CHAR_PARENTHESIS_OPEN = '(';
        private const char CHAR_PARENTHESIS_CLOSE = ')';
        private const char CHAR_DOT = '.';

        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly TextReader m_Reader;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private char CurrentCharacter { get; set; }
        
        public TokenType Type { get; private set; }
        public float Number { get; private set; }

        // INITIALIZER: ---------------------------------------------------------------------------

        public Tokenizer(string expression)
        {
            this.m_Reader = new StringReader(expression);

            this.NextCharacter();
            this.NextToken();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void NextCharacter()
        {
            int character = m_Reader.Read();
            this.CurrentCharacter = character < 0 ? CHAR_EOE : (char) character;
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(this.CurrentCharacter)) this.NextCharacter();
            switch (this.CurrentCharacter)
            {
                case CHAR_EOE:
                    this.Type = TokenType.EndOfExpression;
                    return;

                case CHAR_ADD:
                    this.NextCharacter();
                    this.Type = TokenType.Add;
                    return;

                case CHAR_SUBTRACT:
                    this.NextCharacter();
                    this.Type = TokenType.Subtract;
                    return;

                case CHAR_MULTIPLY:
                    this.NextCharacter();
                    this.Type = TokenType.Multiply;
                    return;

                case CHAR_DIVIDE:
                    this.NextCharacter();
                    this.Type = TokenType.Divide;
                    return;

                case CHAR_PARENTHESIS_OPEN:
                    this.NextCharacter();
                    this.Type = TokenType.OpenParenthesis;
                    return;

                case CHAR_PARENTHESIS_CLOSE:
                    this.NextCharacter();
                    this.Type = TokenType.CloseParenthesis;
                    return;
            }

            if (!char.IsDigit(this.CurrentCharacter) && this.CurrentCharacter != CHAR_DOT)
            {
                throw new Exception($"Unexpected character: {this.CurrentCharacter}");          
            }

            StringBuilder.Clear();
            bool hasFloatingPoint = false;
            
            while (char.IsDigit(this.CurrentCharacter) || 
                   !hasFloatingPoint && this.CurrentCharacter == CHAR_DOT)
            {
                StringBuilder.Append(this.CurrentCharacter);
                hasFloatingPoint = this.CurrentCharacter == CHAR_DOT;
                this.NextCharacter();
            }

            this.Number = float.Parse(StringBuilder.ToString(), Culture);
            this.Type = TokenType.Number;
        }
    }
}