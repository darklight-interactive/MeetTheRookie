using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Utility
{
    public interface IState
    {
        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        void Exit();

        /// <summary>
        /// Called when the state is executed.
        /// </summary>
        /// <remarks>
        /// This method should be called in the game's update loop.
        /// </remarks>
        void Execute();
    }

    public abstract class FiniteState<TState> : IState where TState : Enum
    {
        protected FiniteStateMachine<TState> stateMachine;
        protected TState stateType;
        protected object[] args;

        public TState StateType { get { return stateType; } }

        public FiniteState(TState stateType, params object[] args)
        {
            this.stateType = stateType;
            this.args = args;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Execute();
    }

    /// <summary>
    /// A finite state machine that can be used to manage states in a game.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public abstract class FiniteStateMachine<TState> where TState : Enum
    {
        protected TState initialState;
        protected Dictionary<TState, FiniteState<TState>> stateDictionary;
        protected FiniteState<TState> currentState;
        protected object[] args;

        [SerializeField] private List<FiniteState<TState>> allStates = new List<FiniteState<TState>>();

        public delegate void OnStateChange(TState state);
        public event OnStateChange OnStateChanged;
        public TState CurrentState { get { return currentState.StateType; } }

        public FiniteStateMachine() { }
        public FiniteStateMachine(Dictionary<TState, FiniteState<TState>> stateDictionary, TState initialState, object[] args)
        {
            this.initialState = initialState;
            this.stateDictionary = stateDictionary;
            this.currentState = stateDictionary[initialState];
            this.args = args;

            allStates = stateDictionary.Values.ToList();
        }

        public virtual void Step()
        {
            if (currentState != null) { currentState.Execute(); }
            else { GoToState(initialState); }
        }

        /// <summary>
        /// Change the current state of the state machine.
        /// </summary>
        /// <returns> True if the stat exists and was successfully changed. </returns>
        public virtual bool GoToState(TState state)
        {
            // Exit from the previous state
            if (currentState != null && currentState.StateType.Equals(state)) { return false; }
            if (currentState != null) { currentState.Exit(); }

            // Check if the state exists
            if (stateDictionary.ContainsKey(state))
            {
                // Enter the new state
                currentState = stateDictionary[state];
                currentState.Enter();
            }
            else
            {
                Debug.LogError($"State {state} not found in possible states.");
                return false;
            }

            // Invoke the OnStateChanged event
            OnStateChanged?.Invoke(state);
            return true;
        }
    }
}