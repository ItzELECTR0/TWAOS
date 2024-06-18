using System.Globalization;
using System.Text.RegularExpressions;

namespace GameCreator.Runtime.Common
{
    public static class TextUtils
    {
        private static readonly Regex RX_VAR_NAME = new Regex(@"[^\p{L}\p{Nd}-_]");
        private static readonly Regex RX_VAR_PATH = new Regex(@"[^\p{L}\p{Nd}-_\/]");

        private static readonly TextInfo TXT = CultureInfo.InvariantCulture.TextInfo;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static string Humanize(string source)
        {
            #if UNITY_EDITOR
            source = UnityEditor.ObjectNames.NicifyVariableName(source);
            #endif
            
            char[] characters = source.ToCharArray();
            for (int i = 0; i < characters.Length; ++i)
            {
                if (characters[i] == '-') characters[i] = ' ';
                if (characters[i] == '_') characters[i] = ' ';
            }

            source = new string(characters);
            source = TXT.ToTitleCase(source);
            
            return source;
        }
        
        public static string Humanize(object source)
        {
            return Humanize(source?.ToString());
        }

        public static string ProcessID(string text, bool isPath = false)
        {
            return isPath 
                ? RX_VAR_PATH.Replace(text, "-") 
                : RX_VAR_NAME.Replace(text, "-");
        }

        public static string ProcessScriptName(string text)
        {
            return text.Replace(" ", string.Empty);
        }

        public static string ProcessNamespace(string text)
        {
            text = text.Replace(".", " ");
            text = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text);

            string[] split = text.Split(' ');
            return split[^1];
        }
    }   
}
