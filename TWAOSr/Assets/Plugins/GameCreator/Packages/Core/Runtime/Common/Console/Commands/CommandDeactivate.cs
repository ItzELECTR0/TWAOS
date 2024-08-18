using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandDeactivate : Command
    {
        public override string Name => "deactivate";

        public override string Description => "Sets a game object provided by its parameter as inactive";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandDeactivate() : base(new ActionGameObjectsCollection().Get)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Output[] Run(Input input) => this.RunDefault(input, Operation);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Output Operation(GameObject gameObject)
        {
            if (gameObject == null) return Output.Error("Unable to find game object");

            gameObject.SetActive(false);
            Output result = Output.Success($"Game Object '{gameObject.name}' = inactive");
            
            return result;
        }
    }
}