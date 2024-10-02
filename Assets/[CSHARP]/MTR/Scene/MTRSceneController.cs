using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public enum MTRSceneState
{
    LOADING,
    INITIALIZE,
    ENTER,
    PLAY_MODE,
    CINEMA_MODE,
    PAUSE_MODE,
    SYNTHESIS_MODE,
    EXIT,
    CHOICEMODE
}

[RequireComponent(typeof(MTRSceneManager))]
public class MTRSceneController : MonoBehaviour
{
    InternalStateMachine _stateMachine;
    [SerializeField, ShowOnly] MTRSceneState _currentState;

    public MTR_UIManager UIManager => MTR_UIManager.Instance;
    public MTRSceneTransitionController TransitionController => UIManager.SceneTransitionController;
    public MTRPlayerController PlayerController => MTRGameManager.PlayerController;
    public MTRCameraController CameraController => MTRGameManager.CameraController;
    public InternalStateMachine StateMachine => _stateMachine;
    public MTRSceneState CurrentState => _currentState;

    void Awake()
    {
        _stateMachine = new InternalStateMachine(this);
        _stateMachine.OnStateChanged += OnSceneStateChanged;
    }

    void Start()
    {
        _stateMachine.GoToState(MTRSceneState.INITIALIZE);
    }

    void Update()
    {
        _stateMachine.Step();
    }

    void OnSceneStateChanged(MTRSceneState state)
    {
        Debug.Log($"Scene State Changed: {state}");
        _currentState = state;
    }

    public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        Debug.Log($"Active Scene Changed: {oldScene.name} -> {newScene.name}");

        StateMachine.GoToState(MTRSceneState.INITIALIZE);
    }

    #region ( InternalStateMachine ) ================================================================
    public class InternalStateMachine : FiniteStateMachine<MTRSceneState>
    {
        MTRSceneController _controller;

        public InternalStateMachine(MTRSceneController controller)
        {
            _controller = controller;
            possibleStates = new Dictionary<MTRSceneState, FiniteState<MTRSceneState>>()
            {
                { MTRSceneState.INITIALIZE, new InitializeState(this, MTRSceneState.INITIALIZE) },
                { MTRSceneState.ENTER, new EnterState(this, MTRSceneState.ENTER) },
                { MTRSceneState.PLAY_MODE, new PlayModeState(this, MTRSceneState.PLAY_MODE) },
                { MTRSceneState.CINEMA_MODE, new CinemaModeState(this, MTRSceneState.CINEMA_MODE) },
                { MTRSceneState.PAUSE_MODE, new PauseModeState(this, MTRSceneState.PAUSE_MODE) },
                { MTRSceneState.SYNTHESIS_MODE, new SynthesisModeState(this, MTRSceneState.SYNTHESIS_MODE) },
                { MTRSceneState.EXIT, new ExitState(this, MTRSceneState.EXIT) },
                { MTRSceneState.LOADING, new LoadingState(this, MTRSceneState.LOADING) },
                { MTRSceneState.CHOICEMODE, new ChoiceModeState(this, MTRSceneState.CHOICEMODE) },
            };
            initialState = MTRSceneState.INITIALIZE;
        }

        #region ================== [ BASE STATE ] ==================
        public abstract class BaseState : FiniteState<MTRSceneState>
        {
            protected MTRSceneController sceneController;
            protected InternalStateMachine stateMachine;
            protected MTRCameraController cameraController => MTRGameManager.CameraController;
            protected MTRPlayerController playerController => MTRGameManager.PlayerController;
            protected MTRSceneTransitionController transitionController
            {
                get
                {
                    MTRSceneTransitionController controller = sceneController.UIManager.SceneTransitionController;
                    if (controller == null)
                        controller = FindFirstObjectByType<MTRSceneTransitionController>();
                    return controller;
                }
            }
            public BaseState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType)
            {
                this.stateMachine = stateMachine;
                this.sceneController = stateMachine._controller;
            }
        }
        #endregion

        #region ================== [ LOADING STATE ] ==================
        public class LoadingState : BaseState
        {
            public LoadingState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ================== [ INITIALIZE STATE ] ==================
        public class InitializeState : BaseState
        {
            public InitializeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                if (cameraController.Rig.FollowTarget == null)
                    cameraController.SetPlayerAsFollowTarget();
            }
            public override void Execute()
            {
                stateMachine.GoToState(MTRSceneState.ENTER);
            }
            public override void Exit() { }
        }
        #endregion

        #region ================== [ ENTER STATE ] ==================
        public class EnterState : BaseState
        {
            public EnterState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                sceneController.StartCoroutine(EnterStateCoroutine());
            }
            public override void Execute() { }
            public override void Exit() { }

            IEnumerator EnterStateCoroutine()
            {
                cameraController.SetPlayerAsFollowTarget();
                playerController.Input.SetAllInputsEnabled(true);
                yield return new WaitForSeconds(0.5f);

                //transitionController.StartFadeOut();

                yield return new WaitForSeconds(0.5f);
                stateMachine.GoToState(MTRSceneState.PLAY_MODE);
            }
        }
        #endregion

        #region ================== [ PLAY MODE STATE ] ==================
        public class PlayModeState : BaseState
        {
            public PlayModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }
            public override void Enter()
            {
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ================== [ CINEMA MODE STATE ] ==================
        public class CinemaModeState : BaseState
        {
            public CinemaModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

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
                StateMachine.GoToState(MTRSceneState.PAUSE_MODE);
            }
        }
        #endregion

        #region ================== [ PAUSE MODE STATE ] ==================
        public class PauseModeState : BaseState
        {
            public PauseModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

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
                StateMachine.GoToState(MTRSceneState.SYNTHESIS_MODE);
            }
        }
        #endregion

        #region ================== [ SYNTHESIS MODE STATE ] ==================
        public class SynthesisModeState : BaseState
        {
            public SynthesisModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

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
                StateMachine.GoToState(MTRSceneState.EXIT);
            }
        }
        #endregion

        #region ================== [ EXIT STATE ] ==================
        public class ExitState : BaseState
        {
            public ExitState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

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
                StateMachine.GoToState(MTRSceneState.LOADING);
            }

        }
        #endregion



        #region ================== [ CHOICE MODE STATE ] ==================
        public class ChoiceModeState : BaseState
        {
            public ChoiceModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

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
                StateMachine.GoToState(MTRSceneState.INITIALIZE);
            }
        }
        #endregion



    }
    #endregion

}

