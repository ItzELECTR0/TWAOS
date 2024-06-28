namespace DaimahouGames.Runtime.Core.Common
{
    public class TState : IState
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public FiniteStateMachine StateMachine { get; }
        public bool IsActive => StateMachine.CurrentState == this;
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        
        public TState(FiniteStateMachine ownerFiniteStateMachine) => StateMachine = ownerFiniteStateMachine;
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public void ForceDefaultState() => StateMachine.SetDefaultState();
        public bool CanEnter() => StateMachine.CurrentState != this && StateMachine.CanSetState(this);
        public bool CanReEnter() => StateMachine.CanSetState(this);
        public bool CanExit(IState next) => CanExitState(next);
        
        // ※  Contract Methods: -------------------------------------------------------------------------------------|
        bool IState.CanEnterState(IState previousState) => CanEnterState(previousState);
        bool IState.CanExitState(IState nextState) => CanExitState(nextState);
        void IState.OnEnterState() => OnEnterState();
        void IState.OnExitState() => OnExitState();
        void IState.Update() => Update();
        bool IState.IsOwner(FiniteStateMachine fsm) => StateMachine == fsm;

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected virtual bool CanEnterState(IState previousState) => true;
        protected virtual bool CanExitState(IState nextState) => true;
        protected virtual void OnEnterState() {}
        protected virtual void OnExitState() {}
        protected virtual void Update() {}
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}