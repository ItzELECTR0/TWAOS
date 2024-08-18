using System;

namespace GameCreator.Runtime.Common
{
    public interface IStateMachine
    {
        // EVENTS: --------------------------------------------------------------------------------
        
        event Action<IStateMachine, IState> EventStateEnter;
        event Action<IStateMachine, IState> EventStateExit;
    }
}