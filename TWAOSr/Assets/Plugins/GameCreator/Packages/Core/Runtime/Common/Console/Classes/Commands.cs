using System.Collections.Generic;

namespace GameCreator.Runtime.Console
{
    internal static class Commands
    {
        private const string ERR_NOT_FOUND = "Unable to find command '{0}'";
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static IEnumerable<Output> Run(Input input)
        {
            return Database.Get.TryGetValue(input.Command, out Command value)
                ? value.Run(input)
                : new[] { Output.Error(string.Format(ERR_NOT_FOUND, input.Command), true) };
        }
    }
}