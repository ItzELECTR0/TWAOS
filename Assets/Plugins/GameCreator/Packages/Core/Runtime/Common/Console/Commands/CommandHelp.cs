using System;
using System.Collections.Generic;
using System.Text;

namespace GameCreator.Runtime.Console
{
    public class CommandHelp : Command
    {
        private static readonly string[] HELP_1 = 
        {
            "Input <command> followed by pairs of <parameter> <value>",
            "For example: destroy name Cube",
            "",
            "Commands:"
        };
        
        private static readonly string[] HELP_2 = 
        {
            "Add values between quotes if the value requires a space",
            "For example: destroy name \"My Player\""
        };

        public override string Name => "help";
        public override string Description => string.Empty;

        public override bool IsHidden => true;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Output[] Run(Input input)
        {
            StringBuilder texts = new StringBuilder();
            foreach (string text in HELP_1)
            {
                texts.AppendLine(text);
            }

            List<Command> commands = new List<Command>(Database.Get.Values);
            commands.Sort(CompareCommands);
            
            foreach (Command command in commands)
            {
                if (command.IsHidden) continue;
                
                texts.AppendLine(string.Empty);
                texts.AppendLine($"{command.Name}:");
                
                if (!string.IsNullOrEmpty(command.Description))
                {
                    texts.AppendLine($"  {command.Description}");
                }

                List<IAction> actions = new List<IAction>(command.Actions);
                actions.Sort(CompareActions);
                
                foreach (IAction action in actions)
                {
                    texts.AppendLine($"- {action.Name}: {action.Description}");
                }
            }
            
            texts.AppendLine(string.Empty);
            foreach (string text in HELP_2)
            {
                texts.AppendLine(text);
            }

            return new []
            {
                Output.Success(texts.ToString())
            };
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static int CompareCommands(Command x, Command y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.InvariantCulture);
        }
        
        private static int CompareActions(IAction x, IAction y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.InvariantCulture);
        }
    }
}