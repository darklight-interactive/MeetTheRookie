using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using EasyButtons;
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
    public static string SceneToLoad => Instance._sceneToLoad;

    InternalStateMachine _stateMachine;

    [SerializeField, ShowOnly]
    MTRSceneState _currentSceneState;

    [SerializeField, ShowOnly]
    string _sceneToLoad;

    [SerializeField, ShowOnly]
    int _spawnIndex;

    public MTR_UIManager UIManager => MTR_UIManager.Instance;
    public MTRSceneTransitionController TransitionController => UIManager.SceneTransitionController;
    public MTRPlayerController PlayerController => MTRGameManager.PlayerController;
    public MTRCameraController CameraController => MTRGameManager.CameraController;

    public delegate void SceneStateChangedEvent(MTRSceneState state);
    public event SceneStateChangedEvent OnSceneStateChanged;

    public override void Initialize()
    {
        _stateMachine = new InternalStateMachine(this);
        _stateMachine.OnStateChanged += HandleSceneStateChanged;
        _stateMachine.OnStateChanged += (MTRSceneState state) =>
        {
            OnSceneStateChanged?.Invoke(state);
        };
        _stateMachine.GoToState(MTRSceneState.INITIALIZE);
    }

    void Update()
    {
        _stateMachine.Step();
    }

    void HandleSceneStateChanged(MTRSceneState state)
    {
        Debug.Log($"Scene State Changed: {state}");
        _currentSceneState = state;
    }

    public void TryLoadScene(string sceneName, int spawnIndex = 0)
    {
        if (sceneName == null || sceneName == "")
            return;

        if (sceneName == SceneManager.GetActiveScene().name)
        {
            Debug.Log("Scene is already loaded.");
            return;
        }

        _sceneToLoad = sceneName;
        _spawnIndex = spawnIndex;
        StateMachine.GoToState(MTRSceneState.EXIT);
    }

    public void SetPlayerSpawnPoint(int spawnIndex)
    {
        if (PlayerController == null)
            return;

        // << SET PLAYER SPAWN POINT >>
        MTRInteractionSystem.GetSpawnPointInteractables(out List<MTRInteractable> interactables);

        // if there are spawn points, get the closest valid destination
        if (interactables.Count > 0)
        {
            MTRInteractable spawnPointInteractable = interactables[spawnIndex];

            // << SET PLAYER POSITION >>
            PlayerController.transform.position = new Vector3(
                spawnPointInteractable.transform.position.x,
                PlayerController.transform.position.y,
                0
            );
            Debug.Log(
                $"{PREFIX} {MTRSceneManager.ActiveSceneData.name} :: Player Spawn Point: {spawnPointInteractable.name} ({_spawnIndex})",
                spawnPointInteractable
            );
            /*
            MTRDestinationReciever reciever = null;
            spawnPointInteractable.Recievers.TryGetValue(InteractionType.DESTINATION, out reciever);
            if (reciever != null)
            {
                reciever.GetClosestValidDestination(
                    PlayerController.transform.position,
                    out Vector2 destination
                );
                PlayerController.transform.position = new Vector3(
                    destination.x,
                    PlayerController.transform.position.y,
                    0
                );
                Debug.Log(
                    $"{PREFIX} {MTRSceneManager.ActiveSceneData.name} :: Player Spawn Point: {spawnPointInteractable.name} ({_spawnIndex}) {destination}"
                );
            }
            else
            {
                Debug.LogError(
                    $"{PREFIX} {MTRSceneManager.ActiveSceneData.name} :: No Destination Reciever Found for {spawnPointInteractable.name} ({_spawnIndex})"
                );
            }
            */
        }
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
                {
                    MTRSceneState.SYNTHESIS_MODE,
                    new SynthesisModeState(this, MTRSceneState.SYNTHESIS_MODE)
                },
                { MTRSceneState.EXIT, new ExitState(this, MTRSceneState.EXIT) },
                { MTRSceneState.LOADING, new LoadingState(this, MTRSceneState.LOADING) },
                { MTRSceneState.CHOICEMODE, new ChoiceModeState(this, MTRSceneState.CHOICEMODE) },
            };
            initialState = MTRSceneState.INITIALIZE;
        }

        #region ================== [ BASE STATE ] ==================
        public abstract class BaseState : FiniteState<MTRSceneState>
        {
            protected MTRSceneData activeSceneData => MTRSceneManager.ActiveSceneData;
            protected MTRCameraController cameraController => MTRGameManager.CameraController;
            protected MTRPlayerController playerController => MTRGameManager.PlayerController;
            protected MTRPlayerStateMachine playerStateMachine => playerController?.StateMachine;
            protected MTRSceneTransitionController transitionController
            {
                get
                {
                    MTRSceneTransitionController controller = sceneController
                        .UIManager
                        .SceneTransitionController;
                    if (controller == null)
                        controller = FindFirstObjectByType<MTRSceneTransitionController>();
                    return controller;
                }
            }

            protected InternalStateMachine stateMachine;
            protected MTRSceneController sceneController;

            public BaseState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType)
            {
                this.stateMachine = stateMachine;
                this.sceneController = stateMachine._controller;
            }
        }
        #endregion

        #region ================== [ LOADING STATE ] ==================
        public class LoadingState : BaseState
        {
            public LoadingState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                sceneController.StartCoroutine(LoadSceneAsyncRoutine());
            }

            public override void Execute() { }

            public override void Exit() { }

            IEnumerator LoadSceneAsyncRoutine()
            {
                // Begin loading the scene asynchronously
                UnityEngine.AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(
                    MTRSceneController.SceneToLoad
                );

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
            public InitializeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                if (cameraController != null && cameraController.Rig.FollowTarget == null)
                    cameraController.SetPlayerAsFollowTarget();

                // << GO TO ACTIVE SCENE KNOT >>
                InkyStoryManager.GoToPath(activeSceneData.SceneKnot);
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
            public EnterState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                sceneController.StartCoroutine(EnterStateCoroutine());
            }

            public override void Execute() { }

            public override void Exit() { }

            IEnumerator EnterStateCoroutine()
            {
                // << REFRESH REGISTRY >>
                InteractionSystem.Registry.RefreshRegistry();

                // << SET PLAYER SPAWN POINT >>
                sceneController.SetPlayerSpawnPoint(sceneController._spawnIndex);

                // << SET CAMERA FOLLOW TARGET >>
                cameraController?.SetPlayerAsFollowTarget();

                // << BEGIN TRANSITION >>
                transitionController?.StartWipeOpen();
                yield return new WaitForSeconds(0.5f);

                // << FORCE PLAYER INTERACT WITH ON START INTERACTION STITCH >>
                sceneController.StartCoroutine(ForceInteractionOnEnterCoroutine());
            }

            IEnumerator ForceInteractionOnEnterCoroutine()
            {
                if (activeSceneData.ForceInteractionOnEnter)
                {
                    Debug.Log(
                        $"{PREFIX} {activeSceneData.name} :: Force Interaction On Enter {activeSceneData.OnEnterStitch}"
                    );

                    // << TRY TO GET INTERACTABLE >>
                    // If the interactable is not found, try again up to 3 times with a delay of 0.5 seconds between attempts
                    // This is to account for the fact that the interactable may not be loaded yet
                    const int MAX_ATTEMPTS = 3;
                    const float ATTEMPT_DELAY = 0.5f;
                    MTRInteractable interactable = null;
                    for (int attempt = 1; attempt <= MAX_ATTEMPTS; attempt++)
                    {
                        MTRInteractionSystem.TryGetInteractableByStitch(
                            activeSceneData.OnEnterStitch,
                            out interactable
                        );

                        if (interactable != null)
                        {
                            MTRInteractionSystem.PlayerInteractor?.InteractWith(interactable, true);
                            break;
                        }

                        if (attempt < MAX_ATTEMPTS)
                        {
                            Debug.Log(
                                $"{PREFIX} Attempt {attempt} failed, retrying in {ATTEMPT_DELAY} seconds..."
                            );
                            yield return new WaitForSeconds(ATTEMPT_DELAY);
                        }
                    }

                    if (interactable == null)
                    {
                        Debug.LogError(
                            $"{PREFIX} {activeSceneData.name} :: No Interactable Found for {activeSceneData.OnEnterStitch} after {MAX_ATTEMPTS} attempts"
                        );
                    }

                    yield return new WaitUntil(
                        () => interactable.CurrentState == MTRInteractable.State.COMPLETE
                    );
                }
                stateMachine.GoToState(MTRSceneState.PLAY_MODE);
            }
        }
        #endregion

        #region ================== [ PLAY MODE STATE ] ==================
        public class PlayModeState : BaseState
        {
            public PlayModeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                if (playerStateMachine != null)
                    playerStateMachine.GoToState(MTRPlayerState.FREE_IDLE);
            }

            public override void Execute() { }

            public override void Exit() { }
        }
        #endregion

        #region ================== [ CINEMA MODE STATE ] ==================
        public class CinemaModeState : BaseState
        {
            public CinemaModeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

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
            public PauseModeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                playerController.StateMachine.GoToState(MTRPlayerState.OVERRIDE_IDLE);
            }

            public override void Execute()
            {
                if (playerController.StateMachine.CurrentState != MTRPlayerState.OVERRIDE_IDLE)
                    playerController.StateMachine.GoToState(MTRPlayerState.OVERRIDE_IDLE);
            }

            public override void Exit() { }
        }
        #endregion


        #region ================== [ SYNTHESIS MODE STATE ] ==================
        public class SynthesisModeState : BaseState
        {
            public SynthesisModeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter()
            {
                playerController.StateMachine.GoToState(MTRPlayerState.OVERRIDE_IDLE);
            }

            public override void Exit()
            {
                //playerController.StateMachine.GoToState(MTRPlayerState.FREE_IDLE);
            }

            public override void Execute() { }
        }
        #endregion

        #region ================== [ EXIT STATE ] ==================
        public class ExitState : BaseState
        {
            public ExitState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

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
            public ChoiceModeState(InternalStateMachine stateMachine, MTRSceneState stateType)
                : base(stateMachine, stateType) { }

            public override void Enter() { }

            public override void Execute() { }

            public override void Exit() { }
        }
        #endregion
    }
    #endregion
}
