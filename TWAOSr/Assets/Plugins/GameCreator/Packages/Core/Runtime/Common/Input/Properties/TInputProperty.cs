using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TInputProperty
    {
        // PROPERTIES: ---------------------------------------------------------------------------- 
        
        protected abstract TInput Input { get; }

        // INITIALIZERS: --------------------------------------------------------------------------
        
        public void OnStartup() => this.Input.OnStartup();
        public void OnDispose() => this.Input.OnDispose();
        public void OnUpdate()  => this.Input.OnUpdate();
        
        // STRING: --------------------------------------------------------------------------------

        public abstract override string ToString();
    }
}