using DaimahouGames.Runtime.Core.Common;

namespace DaimahouGames.Runtime.Pawns
{
    public class DefaultState : TPawnState
    {
        public override string Title => "Default State";

        public DefaultState(FiniteStateMachine stateMachine) : base(stateMachine) {}

        protected override void OnEnterState()
        {
            SetControllable(true);
        }

        protected override void OnExitState()
        {
           // SetControllable(false);
           // StopMovement();
        }
    }
}