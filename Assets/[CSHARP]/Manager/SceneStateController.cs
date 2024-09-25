using Darklight.UnityExt.Behaviour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SceneStateController : MonoBehaviour
{
    [NonSerialized] public SceneStateMachine stateMachine;
    public SceneState startingState = SceneState.INITIALIZE;
    public SceneState currentState;

    void Start()
    {
        InitializeState initializeState = new(stateMachine, SceneState.INITIALIZE, new object[] { stateMachine });
        EnterState enterState = new(stateMachine, SceneState.ENTER, new object[] { stateMachine });
        PlayModeState playModeState = new(stateMachine, SceneState.PLAYMODE, new object[] { stateMachine });
        CinemaModeState cinemaModeState = new(stateMachine, SceneState.CINEMAMODE, new object[] { stateMachine });
        PauseModeState pauseModeState = new(stateMachine, SceneState.PAUSEMODE, new object[] { stateMachine });
        SynthesisModeState synthesisModeState = new(stateMachine, SceneState.SYNTHESISMODE, new object[] { stateMachine });
        ExitState exitState = new(stateMachine, SceneState.EXIT, new object[] { stateMachine });
        LoadingState loadingState = new(stateMachine, SceneState.LOADING, new object[] { stateMachine });
        ChoiceModeState choiceModeState = new(stateMachine, SceneState.CHOICEMODE, new object[] { stateMachine });

        // Create dictionary to hold the possible states
        Dictionary<SceneState, FiniteState<SceneState>> possibleStates = new()
        {
            { SceneState.INITIALIZE, initializeState },
            { SceneState.ENTER, enterState },
            { SceneState.PLAYMODE, playModeState },
            { SceneState.CINEMAMODE, cinemaModeState },
            { SceneState.PAUSEMODE, pauseModeState },
            { SceneState.SYNTHESISMODE, synthesisModeState },
            { SceneState.EXIT, exitState },
            { SceneState.LOADING, loadingState },
            { SceneState.CHOICEMODE, choiceModeState },
        };

        stateMachine = new(possibleStates, startingState, this);

        // Hacky solution to fix null reference bug, setting the stateMachine field for each state
        initializeState._stateMachine = stateMachine;
        enterState._stateMachine = stateMachine;
        playModeState._stateMachine = stateMachine;
        cinemaModeState._stateMachine = stateMachine;
        pauseModeState._stateMachine = stateMachine;
        synthesisModeState._stateMachine = stateMachine;
        exitState._stateMachine = stateMachine;
        loadingState._stateMachine = stateMachine;
        choiceModeState._stateMachine = stateMachine;
    }

    void Update()
    {
        stateMachine.Step();
        currentState = stateMachine.CurrentState;
    }
}

public enum SceneState { INITIALIZE, ENTER, PLAYMODE, CINEMAMODE, PAUSEMODE, SYNTHESISMODE, EXIT, LOADING, CHOICEMODE }

public class SceneStateMachine : FiniteStateMachine<SceneState>
{
    public SceneStateController controller;

    public SceneStateMachine(Dictionary<SceneState, FiniteState<SceneState>> possibleStates, SceneState initialState, params object[] args) : base(possibleStates, initialState, args)
    {
        controller = (SceneStateController)args[0];
    }

    public override void Step()
    {
        base.Step();
    }

    public override bool GoToState(SceneState newState, bool force = false)
    {
        bool result = base.GoToState(newState);
        return result;
    }
}

#region ================== [ INITIALIZE STATE ] ==================

public class InitializeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public InitializeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ ENTER STATE ] ==================

public class EnterState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public EnterState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ PLAYMODE STATE ] ==================

public class PlayModeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public PlayModeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ CINEMAMODE STATE ] ==================

public class CinemaModeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public CinemaModeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ PAUSEMODE STATE ] ==================

public class PauseModeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public PauseModeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ SYNTHESISMODE STATE ] ==================

public class SynthesisModeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public SynthesisModeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ EXIT STATE ] ==================

public class ExitState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public ExitState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ LOADING STATE ] ==================

public class LoadingState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public LoadingState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion

#region ================== [ CHOICEMODE STATE ] ==================

public class ChoiceModeState : FiniteState<SceneState>
{
    public SceneStateMachine _stateMachine;
    public ChoiceModeState(SceneStateMachine stateMachine, SceneState stateType, params object[] args) : base(stateMachine, stateType)
    {
        _stateMachine = (SceneStateMachine)args[0];
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Execute() { }
}

#endregion
