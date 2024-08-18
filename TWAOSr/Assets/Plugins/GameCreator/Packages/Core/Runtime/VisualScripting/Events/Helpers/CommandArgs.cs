using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    public readonly struct CommandArgs
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public PropertyName Command { get; }
        
        public GameObject Target { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CommandArgs(PropertyName command)
        {
            this.Command = command;
            this.Target = null;
        }
        
        public CommandArgs(PropertyName command, GameObject target) : this(command)
        {
            this.Target = target;
        }
    }
}