using System.Collections;
using System.Collections.Generic;
using Darklight.Game;
using Darklight.Game.Camera;
using Darklight.Game.Utility;
using UnityEngine;

public enum CameraState
{
    NULL,
    FOLLOW,

}

public class CameraStateMachine : FiniteStateMachine<CameraState>
{
    public CameraStateMachine(CameraState initialState, Dictionary<CameraState, IState<CameraState>> possibleStates, GameObject parent) : base(initialState, possibleStates, parent)
    {

    }
}


public class IdleState : IState<CameraState>
{
    public FiniteStateMachine<CameraState> StateMachine { get; set; }

    public void Enter(params object[] enterArgs)
    {
        Debug.Log("Entering Idle State");
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }

    public void Execute(params object[] executeArgs)
    {
        Debug.Log("Executing Idle State");
    }
}