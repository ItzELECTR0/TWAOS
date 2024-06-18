using System;
using System.Collections;
using System.Collections.Generic;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TVariableRuntime<T> : IEnumerable<T> where T : TVariable
    {
        // METHODS: -------------------------------------------------------------------------------

        public abstract void OnStartup();

        // ENUMERATORS: ---------------------------------------------------------------------------

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}