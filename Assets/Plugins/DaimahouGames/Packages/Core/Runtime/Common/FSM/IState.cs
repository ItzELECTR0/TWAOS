namespace DaimahouGames.Runtime.Core.Common
{
    public interface IState
    {
        bool CanEnterState(IState previousState);
        bool CanExitState(IState nextState);
        void OnEnterState();
        void OnExitState();
        void Update();
        bool IsOwner(FiniteStateMachine fsm);
        bool IsActive { get; }
    }
}