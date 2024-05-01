using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public abstract class FiniteStateMachine<TState> where TState : Enum
{
    protected TState initialState;
    protected Dictionary<TState, IState<TState>> possibleStates;
    public GameObject parent;
    public TState currentState;
    protected IState<TState> state;

    public FiniteStateMachine(TState initialState, Dictionary<TState, IState<TState>> possibleStates, GameObject parent)
    {
        this.initialState = initialState;
        this.possibleStates = possibleStates;
        this.parent = parent;
        state = null;

        // state instances get access to the state machine via `this.stateMachine`
        foreach (var kvp in possibleStates)
        {
            IState<TState> possibleState = kvp.Value;
            possibleState.StateMachine = this;
        }
    }

    public virtual void Step()
    {
        // this method should be called in the update() loop
        // on the first step, the state is null and needs to be initialized
        if (state == null)
        {
            GoToState(initialState);
        }

        possibleStates[currentState].Execute();
    }

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

public interface IState<TState> where TState : Enum
{
    FiniteStateMachine<TState> StateMachine { get; set; }

    void Enter(params object[] enterArgs);
    void Exit();
    void Execute(params object[] executeArgs);    // this code happens each update step (i.e., every frame)
}