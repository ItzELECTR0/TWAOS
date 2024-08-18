using System;

namespace GameCreator.Runtime.Common
{
    [Title("Input Button")]
    
    [Serializable]
    public abstract class TInputButton : TInput
    {
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action EventStart;
        public event Action EventCancel;
        public event Action EventPerform;

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void ExecuteEventStart() => this.EventStart?.Invoke();
        protected void ExecuteEventCancel() => this.EventCancel?.Invoke();
        protected void ExecuteEventPerform() => this.EventPerform?.Invoke();
    }
}