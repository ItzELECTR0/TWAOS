using System;

namespace GameCreator.Runtime.Common
{
    public abstract class TStateMachine : IStateMachine
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] protected IState Current { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<IStateMachine, IState> EventStateEnter;
        public event Action<IStateMachine, IState> EventStateExit;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TStateMachine()
        { }
        
        protected TStateMachine(IState state) : this()
        {
            this.Change(state);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void OnUpdate()
        {
            this.Current?.OnUpdate(this);
        }

        protected void Change(IState state)
        {
            if (this.Current != null)
            {
                this.Current.OnExit(this);
                this.Current.EventOnEnter -= this.OnEnterCallback;
                this.Current.EventOnExit -= this.OnExitCallback;
            }

            this.Current = state;
            if (this.Current != null)
            {
                this.Current.EventOnEnter += this.OnEnterCallback;
                this.Current.EventOnExit += this.OnExitCallback;
                this.Current.OnEnter(this);   
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnEnterCallback(IStateMachine stateMachine, IState state)
        {
            this.EventStateEnter?.Invoke(stateMachine, state);
        }
        
        private void OnExitCallback(IStateMachine stateMachine, IState state)
        {
            this.EventStateExit?.Invoke(stateMachine, state);
        }
    }
}
