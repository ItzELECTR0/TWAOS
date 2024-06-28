using System;
using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public sealed class FiniteStateMachine
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|

        private const string NULL_NOT_ALLOWED = "Null state not allowed.";
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public IState CurrentState { get; private set; }
        public IState DefaultState { get; }

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|


        public FiniteStateMachine(Type defaultState)
        {
            DefaultState = (IState) Activator.CreateInstance(defaultState, this);
            CurrentState = DefaultState;
        }
        
        public FiniteStateMachine(IState defaultState)
        {
            DefaultState = defaultState;
            CurrentState = defaultState ?? throw new ArgumentNullException(nameof(defaultState), NULL_NOT_ALLOWED);
            defaultState.OnEnterState();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|
                
        public T CreateState<T>() where T : class, IState
        {
            return (T) Activator.CreateInstance(typeof(T), this);
        }
        
        public bool CanSetState(IState nextState)
        {
            if (nextState == null) throw new ArgumentNullException(nameof(nextState), NULL_NOT_ALLOWED);

            return CurrentState.CanExitState(nextState) && nextState.CanEnterState(CurrentState);
        }
        
        public bool TrySetState(IState state)
        {
            return CurrentState == state || TryResetState(state);
        }
        
        public bool TryResetState(IState state)
        {
            if (!CanSetState(state)) return false;
            
            ForceSetState(state);
            return true;
        }
        
        public void SetDefaultState()
        {
            ForceSetState(DefaultState);
        }
        
        public async void ForceSetState(IState state)
        {
            switch (state)
            {
                case null:
                    throw new ArgumentNullException(nameof(state), NULL_NOT_ALLOWED);
                case var owned when !owned.IsOwner(this) :
                    throw new InvalidOperationException(
                        $"Attempted to use a state in a machine that is not its owner." +
                        $"\n    State: {state}" +
                        $"\n    Machine: {this}");
            }

            CurrentState?.OnExitState();
            CurrentState = state;

            await Awaiters.NextFrame;
            
            state?.OnEnterState();
        }

        /// <summary>Returns a string describing the type of this state machine and its <see cref="CurrentState"/>.</summary>
        public override string ToString() => $"{GetType().Name} -> {CurrentState}";

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        //============================================================================================================||
    }
}