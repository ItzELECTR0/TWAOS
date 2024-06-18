using System.Text.RegularExpressions;

namespace GameCreator.Editor.Search
{
    internal static class Tokenizer
    {
        private static readonly Regex Separator = new Regex(@"[\s\-]+");
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static string[] Run(string text)
        {
            string[] subTexts = Separator.Split(text);
            string[] tokens = new string[subTexts.Length];

            for (int i = 0; i < subTexts.Length; ++i)
            {
                tokens[i] = subTexts[i].ToLowerInvariant();
            }

            return tokens;
        }
    }
}