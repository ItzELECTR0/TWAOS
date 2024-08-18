using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public struct SignalArgs
    {
        public readonly PropertyName signal;
        public readonly GameObject invoker;

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public SignalArgs(PropertyName signal, GameObject invoker)
        {
            this.signal = signal;
            this.invoker = invoker;
        }
    }
}