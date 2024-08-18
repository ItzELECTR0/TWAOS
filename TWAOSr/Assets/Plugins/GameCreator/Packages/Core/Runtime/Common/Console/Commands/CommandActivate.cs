using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandActivate : Command
    {
        public override string Name => "activate";

        public override string Description => "Sets a game object provided by its parameter as active";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandActivate() : base(new ActionGameObjectsCollection().Get)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Output[] Run(Input input) => this.RunDefault(input, Operation);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Output Operation(GameObject gameObject)
        {
            if (gameObject == null) return Output.Error("Unable to find game object");

            gameObject.SetActive(true);
            Output result = Output.Success($"Game Object '{gameObject.name}' = active");
            
            return result;
        }
    }
}