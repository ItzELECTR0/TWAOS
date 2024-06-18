using System;

namespace GameCreator.Runtime.Common
{
    public interface IState
    {
        // EVENTS: --------------------------------------------------------------------------------
        
        event Action<IStateMachine, IState> EventOnEnter;
        event Action<IStateMachine, IState> EventOnExit;
        
        // METHODS: -------------------------------------------------------------------------------
        
        void OnEnter(IStateMachine stateMachine);
        void OnExit(IStateMachine stateMachine);
        void OnUpdate(IStateMachine stateMachine);
    }
}