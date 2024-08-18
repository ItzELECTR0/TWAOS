using System;
using System.Linq;
using System.Text;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true
    )]
    public class ExampleAttribute : Attribute
    {
        public string Content { get; }

        public ExampleAttribute(string content)
        {
            int whiteSpaces = content.Trim(Environment.NewLine[0]).TakeWhile(Char.IsWhiteSpace).Count();
            StringBuilder pattern = new StringBuilder(Environment.NewLine);
            for (int i = 0; i < whiteSpaces; ++i) pattern.Append(' ');

            content = content.Replace(pattern.ToString(), Environment.NewLine);
            this.Content = content.Trim(Environment.NewLine[0], ' ');
        }
    }
}
