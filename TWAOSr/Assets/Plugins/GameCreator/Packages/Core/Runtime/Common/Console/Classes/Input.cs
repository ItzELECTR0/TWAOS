using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCreator.Runtime.Console
{
    public class Input
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public string Command { get; }
        [field: NonSerialized] public Parameter[] Parameters { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private Input()
        {
            this.Command = string.Empty;
            this.Parameters = Array.Empty<Parameter>();
        }
        
        public Input(string command, Parameter[] parameters) : this()
        {
            this.Command = command;
            this.Parameters = parameters ?? Array.Empty<Parameter>();
        }

        public Input(string input) : this()
        {
            List<string> texts = input
                .Split(Parameter.QUOTES)
                .Select((element, index) => index % 2 == 0
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    : new[] { element })
                .SelectMany(element => element).ToList();

            if (texts.Count <= 0) return;
            
            this.Command = texts[0].ToLowerInvariant();
            
            List<Parameter> parameters = new List<Parameter>();
            for (int i = 1; i < texts.Count; i += 2)
            {
                Parameter parameter = new Parameter(
                    texts[i],
                    i + 1 < texts.Count 
                        ? texts[i + 1] 
                        : string.Empty
                );

                parameters.Add(parameter);
            }

            this.Parameters = parameters.ToArray();
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            StringBuilder text = new StringBuilder($"> {this.Command}");
            foreach (Parameter parameter in this.Parameters)
            {
                text.Append(" ");
                text.Append(parameter);
            }

            return text.ToString();
        }
    }
}