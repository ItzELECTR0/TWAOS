using System;

namespace GameCreator.Runtime.Common
{
    public abstract class TState : IState
    {
        public event Action<IStateMachine, IState> EventOnEnter;
        public event Action<IStateMachine, IState> EventOnExit;
        public event Action<IStateMachine, IState> EventOnBeforeUpdate;

        // PROPERTIES: ----------------------------------------------------------------------------
            
        [field: NonSerialized] public bool IsActive { get; private set; }
            
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void OnEnter(IStateMachine stateMachine)
        {
            this.IsActive = true;
            this.WhenEnter(stateMachine);
            this.EventOnEnter?.Invoke(stateMachine, this);   
        }

        public void OnExit(IStateMachine stateMachine)
        {
            this.IsActive = false;
            this.WhenExit(stateMachine);
            this.EventOnExit?.Invoke(stateMachine, this);   
        }

        public void OnUpdate(IStateMachine stateMachine)
        {
            this.WhenUpdate(stateMachine);
            this.EventOnBeforeUpdate?.Invoke(stateMachine, this);   
        }
            
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void WhenEnter(IStateMachine stateMachine)
        { }
            
        protected virtual void WhenExit(IStateMachine stateMachine)
        { }
        
        protected virtual void WhenUpdate(IStateMachine stateMachine)
        { }
    }
}
