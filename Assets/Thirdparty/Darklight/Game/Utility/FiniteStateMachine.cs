using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Utility
{
    /// <summary>
    /// Defines an interface for a state in a finite state machine (FSM).
    /// </summary>
    /// <typeparam name="TState">The type of state, which must be an enumeration.</typeparam>
    public interface IState<TState> where TState : Enum
    {
        /// <summary>
        /// Gets or sets the state machine to which this state belongs.
        /// </summary>
        FiniteStateMachine<TState> StateMachine { get; set; }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        /// <param name="enterArgs">Optional parameters passed to the state upon entry.</param>
        void Enter(params object[] enterArgs);

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        void Exit();

        /// <summary>
        /// Called every frame while the state is active.
        /// </summary>
        /// <param name="executeArgs">Optional parameters passed to the state during execution.</param>
        void Execute(params object[] executeArgs);
    }

    /// <summary>
    /// An abstract class implementing a finite state machine (FSM) for managing state transitions.
    /// </summary>
    /// <typeparam name="TState">The type of states, which must be an enumeration.</typeparam>
    public abstract class FiniteStateMachine<TState> where TState : Enum
    {
        protected TState initialState;

        /// <summary>
        /// A dictionary mapping state identifiers to corresponding state objects.
        /// </summary>
        protected Dictionary<TState, IState<TState>> possibleStates;
        public List<TState> PossibleStates => new List<TState>(possibleStates.Keys);

        /// <summary>
        /// The current state identifier.
        /// </summary>
        protected TState currentState;

        /// <summary>
        /// The current state object.
        /// </summary>
        protected IState<TState> state;

        public GameObject parentObject { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine{TState}"/> class.
        /// </summary>
        /// <param name="initialState">The initial state of the state machine.</param>
        /// <param name="possibleStates">A dictionary mapping state identifiers to state objects.</param>
        /// <param name="parent">The parent GameObject associated with the state machine.</param>
        public FiniteStateMachine(TState initialState, Dictionary<TState, IState<TState>> possibleStates, GameObject parent)
        {
            this.initialState = initialState;
            this.possibleStates = possibleStates;
            this.parentObject = parent;
            state = null;

            // State instances get access to the state machine via `this.StateMachine`
            foreach (KeyValuePair<TState, IState<TState>> kvp in possibleStates)
            {
                IState<TState> possibleState = kvp.Value;
                possibleState.StateMachine = this;
            }
        }

        /// <summary>
        /// Performs a step in the state machine's current state.
        /// </summary>
        /// <remarks>
        /// This method should be called in the game's update loop.
        /// </remarks>
        public virtual void Step()
        {
            // On the first step, the state is null and needs to be initialized.
            if (state == null)
            {
                GoToState(initialState);
            }

            possibleStates[currentState]?.Execute();
        }

        /// <summary>
        /// Transitions the state machine to the specified state.
        /// </summary>
        /// <param name="state">The state to transition to.</param>
        /// <param name="enterArgs">Optional parameters passed to the state upon entry.</param>
        public virtual void GoToState(TState state, params object[] enterArgs)
        {
            if (this.state != null)
            {
                this.state.Exit();
            }
            this.state = possibleStates[state];
            currentState = state;
            this.state.Enter(enterArgs);
        }
    }





}