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
public class MTRSceneController : MonoBehaviourSingleton<MTRSceneController>
{
    const string PREFIX = "[MTRSceneController]";
    public static InternalStateMachine StateMachine => Instance._stateMachine;


    InternalStateMachine _stateMachine;
    [SerializeField, ShowOnly] MTRSceneState _currentSceneState;

    public MTR_UIManager UIManager => MTR_UIManager.Instance;
    public MTRSceneTransitionController TransitionController => UIManager.SceneTransitionController;
    public MTRPlayerController PlayerController => MTRGameManager.PlayerController;
    public MTRCameraController CameraController => MTRGameManager.CameraController;

    public override void Initialize()
    {
        _stateMachine = new InternalStateMachine(this);
        _stateMachine.OnStateChanged += OnSceneStateChanged;
        _stateMachine.GoToState(MTRSceneState.INITIALIZE);
    }

    void Update()
    {
        _stateMachine.Step();
    }

    void OnSceneStateChanged(MTRSceneState state)
    {
        Debug.Log($"Scene State Changed: {state}");
        _currentSceneState = state;
    }

    public void TryLoadScene(string sceneName)
    {
        if (sceneName == null || sceneName == "")
            return;

        if (sceneName == SceneManager.GetActiveScene().name)
        {
            Debug.Log("Scene is already loaded.");
            return;
        }

        StateMachine.GoToState(MTRSceneState.EXIT);
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
            protected MTRSceneManager sceneManager => MTRSceneManager.Instance;
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

            protected InternalStateMachine stateMachine;
            protected MTRSceneController sceneController;

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

            public override void Enter()
            {
                sceneController.StartCoroutine(LoadSceneAsyncRoutine());
            }
            public override void Execute() { }
            public override void Exit() { }

            IEnumerator LoadSceneAsyncRoutine()
            {
                // Begin loading the scene asynchronously
                UnityEngine.AsyncOperation asyncOperation =
                    SceneManager.LoadSceneAsync(MTRSceneManager.SceneToLoad);

                // Prevent the scene from being activated immediately
                asyncOperation.allowSceneActivation = false;

                // While the scene is still loading
                while (!asyncOperation.isDone)
                {
                    // Output the current progress (0 to 0.9)
                    float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                    Debug.Log("Loading progress: " + (progress * 100) + "%");

                    // If the loading is almost done (progress >= 90%), allow activation
                    if (asyncOperation.progress >= 0.9f)
                    {
                        asyncOperation.allowSceneActivation = true;

                        yield return new WaitForSeconds(0.5f);
                        stateMachine.GoToState(MTRSceneState.INITIALIZE);
                    }

                    yield return null;
                }
            }
        }
        #endregion

        #region ================== [ INITIALIZE STATE ] ==================
        public class InitializeState : BaseState
        {
            public InitializeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }

            public override void Enter()
            {
                if (cameraController != null && cameraController.Rig.FollowTarget == null)
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
                cameraController?.SetPlayerAsFollowTarget();
                yield return new WaitForSeconds(2f);

                //transitionController.StartFadeIn();
                transitionController?.StartWipeOpen();
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
                sceneController.StartCoroutine(ExitStateCoroutine());
            }
            public override void Execute() { }
            public override void Exit() { }

            IEnumerator ExitStateCoroutine()
            {
                transitionController.StartWipeClose();

                yield return new WaitForSeconds(2f);
                stateMachine.GoToState(MTRSceneState.LOADING);
            }

        }
        #endregion



        #region ================== [ CHOICE MODE STATE ] ==================
        public class ChoiceModeState : BaseState
        {
            public ChoiceModeState(InternalStateMachine stateMachine, MTRSceneState stateType) : base(stateMachine, stateType) { }
            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion



    }
    #endregion

}

