using Darklight.UnityExt.Behaviour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MTRSceneController : MonoBehaviour
{
    public enum SceneState { INITIALIZE, ENTER, PLAYMODE, CINEMAMODE, PAUSEMODE, SYNTHESISMODE, EXIT, LOADING, CHOICEMODE }

    [NonSerialized] public SceneStateMachine stateMachine;
    public SceneState startingState = SceneState.INITIALIZE;
    public SceneState currentState;

    void Start()
    {
        stateMachine = new SceneStateMachine();
        stateMachine.GoToState(startingState);
    }

    void Update()
    {
        stateMachine.Step();
        currentState = stateMachine.CurrentState;
    }

    #region ( Scene State Machine ) ================================================================
    public class SceneStateMachine : FiniteStateMachine<SceneState>
    {
        MTRSceneController _controller;

        public SceneStateMachine()
        {
            possibleStates = new Dictionary<SceneState, FiniteState<SceneState>>()
            {
                { SceneState.INITIALIZE, new InitializeState(this, SceneState.INITIALIZE) },
                { SceneState.ENTER, new EnterState(this, SceneState.ENTER) },
                { SceneState.PLAYMODE, new PlayModeState(this, SceneState.PLAYMODE) },
                { SceneState.CINEMAMODE, new CinemaModeState(this, SceneState.CINEMAMODE) },
                { SceneState.PAUSEMODE, new PauseModeState(this, SceneState.PAUSEMODE) },
                { SceneState.SYNTHESISMODE, new SynthesisModeState(this, SceneState.SYNTHESISMODE) },
                { SceneState.EXIT, new ExitState(this, SceneState.EXIT) },
                { SceneState.LOADING, new LoadingState(this, SceneState.LOADING) },
                { SceneState.CHOICEMODE, new ChoiceModeState(this, SceneState.CHOICEMODE) },
            };
        }

        #region ================== [ BASE STATE ] ==================
        public abstract class BaseState : FiniteState<SceneState>
        {
            public BaseState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }
        }
        #endregion

        #region ================== [ INITIALIZE STATE ] ==================
        public class InitializeState : BaseState
        {
            public InitializeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Initialize State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Initialize State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.ENTER);
            }
        }
        #endregion

        #region ================== [ ENTER STATE ] ==================
        public class EnterState : BaseState
        {
            public EnterState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Enter State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Enter State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.PLAYMODE);
            }
        }
        #endregion

        #region ================== [ PLAY MODE STATE ] ==================
        public class PlayModeState : BaseState
        {
            public PlayModeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Play Mode State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Play Mode State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.CINEMAMODE);
            }
        }
        #endregion

        #region ================== [ CINEMA MODE STATE ] ==================
        public class CinemaModeState : BaseState
        {
            public CinemaModeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Cinema Mode State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Cinema Mode State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.PAUSEMODE);
            }
        }
        #endregion

        #region ================== [ PAUSE MODE STATE ] ==================
        public class PauseModeState : BaseState
        {
            public PauseModeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Pause Mode State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Pause Mode State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.SYNTHESISMODE);
            }
        }
        #endregion

        #region ================== [ SYNTHESIS MODE STATE ] ==================
        public class SynthesisModeState : BaseState
        {
            public SynthesisModeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Synthesis Mode State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Synthesis Mode State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.EXIT);
            }
        }
        #endregion

        #region ================== [ EXIT STATE ] ==================
        public class ExitState : BaseState
        {
            public ExitState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Exit State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Exit State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.LOADING);
            }
        }
        #endregion

        #region ================== [ LOADING STATE ] ==================
        public class LoadingState : BaseState
        {
            public LoadingState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Loading State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Loading State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.CHOICEMODE);
            }
        }
        #endregion

        #region ================== [ CHOICE MODE STATE ] ==================
        public class ChoiceModeState : BaseState
        {
            public ChoiceModeState(SceneStateMachine stateMachine, SceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                Debug.Log("Choice Mode State Enter");
            }

            public override void Exit()
            {
                Debug.Log("Choice Mode State Exit");
            }

            public override void Execute()
            {
                StateMachine.GoToState(SceneState.INITIALIZE);
            }
        }
        #endregion



    }
    #endregion

}

