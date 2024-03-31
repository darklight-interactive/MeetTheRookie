using System;
using System.Collections.Generic;

namespace Darklight
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
        protected StateMachine(TState initialState)
        {
            _currentState = initialState;
        }
        /// <summary>
        /// Update the current state of the machine
        /// </summary>
        protected void ChangeState(TState newState)
        {
            CurrentState = newState;
        }

        public virtual void OnStateChanged(TState previousState, TState newState)
        {
            //Console.WriteLine($"Transitioned from {previousState} to {newState}");
        }
    }
}