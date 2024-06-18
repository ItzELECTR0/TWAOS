using System;

namespace GameCreator.Runtime.Console
{
    public class Output
    {
        private const string ERR_HELP = "Type 'help' to see a list of all available commands.";
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public bool IsError { get; }
        [field: NonSerialized] public string Text { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private Output(bool isError, string text)
        {
            this.IsError = isError;
            this.Text = text;
        }

        public static Output Error(string error, bool showHelp = false)
        {
            return showHelp
                ? new Output(true, $"{error}. {ERR_HELP}")
                : new Output(true, $"{error}.");
        }
        
        public static Output Success(string text)
        {
            return new Output(false, text);
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => this.Text;
    }
}