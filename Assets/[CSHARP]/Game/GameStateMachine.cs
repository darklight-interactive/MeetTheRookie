using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameStateMachine : StateMachine<GameState>
{
    public GameStateMachine(GameState baseState) : base(baseState) { }
    public override void ChangeState(GameState newState)
    {
        base.ChangeState(newState);
    }

    public override void OnStateChanged(GameState previousState, GameState newState)
    {
        base.OnStateChanged(previousState, newState);
    }
}

