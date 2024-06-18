using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandVisualScripting : Command
    {
        public override string Name => "run";

        public override string Description => "Executes a Trigger, Conditions or Actions";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandVisualScripting() : base(new ActionGameObjectsCollection().Get)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Output[] Run(Input input) => this.RunDefault(input, Operation);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Output Operation(GameObject gameObject)
        {
            if (gameObject == null) return Output.Error("Unable to find game object");

            Args args = new Args(gameObject);
            
            Actions actions = gameObject.GetComponent<Actions>();
            if (actions != null)
            {
                _ = actions.Run(args);
                return Output.Success($"Run Actions on '{gameObject.name}'");
            }
            
            Trigger trigger = gameObject.GetComponent<Trigger>();
            if (trigger != null)
            {
                _ = trigger.Execute(args);
                return Output.Success($"Run Trigger on '{gameObject.name}'");
            }
            
            Conditions conditions = gameObject.GetComponent<Conditions>();
            if (conditions != null)
            {
                _ = conditions.Run(args);
                return Output.Success($"Run Conditions on '{gameObject.name}'");
            }
            
            return Output.Error($"Could not find anything to run on '{gameObject.name}'");
        }
    }
}