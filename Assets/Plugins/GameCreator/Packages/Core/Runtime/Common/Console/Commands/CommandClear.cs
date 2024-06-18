using System;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandClear : Command
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Name => "clear";

        public override string Description => "Clears the Console";

        public override bool IsHidden => true;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Output[] Run(Input input)
        {
            Console.Clear();
            return Array.Empty<Output>();
        }
    }
}