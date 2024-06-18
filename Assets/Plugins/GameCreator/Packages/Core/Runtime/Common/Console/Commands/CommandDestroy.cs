using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandDestroy : Command
    {
        public override string Name => "destroy";

        public override string Description => "Destroys a game object provided by its parameter";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandDestroy() : base(new ActionGameObjectsCollection().Get)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Output[] Run(Input input) => this.RunDefault(input, Operation);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Output Operation(GameObject gameObject)
        {
            if (gameObject == null) return Output.Error("Unable to find game object");

            Output result = Output.Success($"Destroy Game Object '{gameObject.name}'");
            Object.Destroy(gameObject);
            
            return result;
        }
    }
}