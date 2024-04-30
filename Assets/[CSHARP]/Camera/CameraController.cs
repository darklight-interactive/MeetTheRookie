using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using static Darklight.UnityExt.CustomInspectorGUI;


#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteAlways]
public class CameraController : CameraRig3D
{

    [Space(10), Header("Camera Controller")]
    [SerializeField] private Transform _defaultLookTarget;
    [SerializeField] private CameraStateMachine _stateMachine;

    [Header("Settings")]
    [SerializeField] CameraSettings _defaultSettings;
    [SerializeField] CameraSettings _followTargetSettings;
    [SerializeField] CameraSettings _closeUpSettings;

    public void Awake()
    {
        _stateMachine = new CameraStateMachine
        (
            CameraState.DEFAULT,
            new Dictionary<CameraState, IState<CameraState>>()
            {
                { CameraState.DEFAULT, new SettingsChangeState(this, ref _defaultSettings) },
                { CameraState.FOLLOW_TARGET, new SettingsChangeState(this, ref _followTargetSettings) },
                { CameraState.CLOSE_UP, new SettingsChangeState(this, ref _closeUpSettings) }
            },
            gameObject
        );
    }

    protected override void Update()
    {
        base.Update(); // Call the base Update method

        // Find the Player Controller
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null) return;

        // Change the Camera State based on the Player State
        PlayerStateMachine playerStateMachine = playerController.stateMachine;
        IInteract activeInteractable = playerController.playerInteractor.ActiveInteractable;

        // >> Switch Case for Player State
        switch (playerStateMachine.CurrentState)
        {
            case PlayerState.IDLE:
            case PlayerState.WALK:

                LookTargetPosition = _defaultLookTarget.position;
                _stateMachine.GoToState(CameraState.FOLLOW_TARGET);
                break;

            case PlayerState.INTERACTION:

                Interactable interactable = activeInteractable as Interactable;
                if (interactable is NPC_Interactable)
                {
                    Vector3 interactableBestPosition = interactable.GetBestData().worldPosition;
                    Vector3 defaultLookTargetPosition = _defaultLookTarget.position;

                    // Set the Look Target Position to the Midpoint between the Interactable and the Default Look Target
                    Vector3 midpoint = (defaultLookTargetPosition + interactableBestPosition) / 2;
                    LookTargetPosition = midpoint;
                }
                else if (interactable is Clue_Interactable)
                {
                    PlayerDialogueHandler dialogueHandler = playerController.GetComponent<PlayerDialogueHandler>();
                    Vector3 bestPosition = dialogueHandler.GetBestData().worldPosition;

                    Vector3 midpoint = (_defaultLookTarget.position + bestPosition) / 2;
                    LookTargetPosition = midpoint;
                }

                _stateMachine.GoToState(CameraState.CLOSE_UP);
                break;
        }
    }
}

// ============================ CAMERA STATE MACHINE ============================ >>

public enum CameraState
{
    DEFAULT,
    FOLLOW_TARGET,
    CLOSE_UP
}

[System.Serializable]
public class CameraStateMachine : FiniteStateMachine<CameraState>
{
    [SerializeField, ShowOnly] private CameraState _currentState;
    public CameraState CurrentState
    {
        get
        {
            _currentState = base.currentState;
            return _currentState;
        }
    }

    public CameraStateMachine
    (
        CameraState initialState,
        Dictionary<CameraState, IState<CameraState>> possibleStates,
        GameObject parent
    ) : base(initialState, possibleStates, parent) { }

    public override void GoToState(CameraState state, params object[] enterArgs)
    {
        // Return if the current state is the same as the target state
        if (currentState == state) return;
        base.GoToState(state, enterArgs);
    }
}

/// <summary>
/// This state is the default state for the camera. It does not follow any target and is in a fixed position.
/// </summary>
public class SettingsChangeState : IState<CameraState>
{
    CameraRig3D _rig3D;
    CameraSettings _settings;
    public SettingsChangeState(CameraRig3D rig3D, ref CameraSettings settings)
    {
        _settings = settings;
        _rig3D = rig3D;
    }

    public FiniteStateMachine<CameraState> StateMachine { get; set; }

    public void Enter(params object[] enterArgs)
    {
        _rig3D.ActiveSettings = _settings;
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public void Execute(params object[] executeArgs)
    {
        //throw new System.NotImplementedException();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    void OnEnable()
    {
        CameraController cameraController = (CameraController)target;
        cameraController.Awake();
    }
}
#endif