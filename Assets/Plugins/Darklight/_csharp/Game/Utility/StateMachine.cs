using System;
using System.Collections.Generic;

namespace Darklight.Game
{
    public abstract class StateMachine<TState> where TState : Enum
    {
        private TState _currentState;
        public TState CurrentState
        {
            get => _currentState;
            private set
            {
                if (!EqualityComparer<TState>.Default.Equals(_currentState, value))
                {
                    TState previousState = _currentState;
                    _currentState = value;
                    OnStateChanged(previousState, _currentState);
                }
            }
        }

        /// <summary>
        /// Assigns the initial state when the class is created
        /// </summary>
        public StateMachine(TState initialState)
        {
            GoToState(initialState);
        }
        /// <summary>
        /// Update the current state of the machine
        /// </summary>
        public virtual void GoToState(TState newState)
        {
            if (newState.Equals(CurrentState)) return;
            CurrentState = newState;
        }

        public virtual void OnStateChanged(TState previousState, TState newState) { }
    }
}