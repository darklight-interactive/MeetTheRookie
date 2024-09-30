using System.Collections.Generic;
using Darklight.UnityExt.Game.Camera;
using Darklight.UnityExt.Behaviour;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteAlways]
public class PlayerCameraController : CameraController
{

    MTRPlayerInputController _playerController;

    [Header(">> Player Camera State Adjustments")]
    [SerializeField, Range(-1, 1)] private float defaultStateFOVOffset = 0f;
    [SerializeField, Range(-1, 1)] private float followTargetStateFOVOffset = -0.25f;
    [SerializeField, Range(-1, 1)] private float closeUpStateFOVOffset = -0.5f;

    public void Awake()
    {
        _playerController = FindFirstObjectByType<MTRPlayerInputController>();
        SetFocusTarget(_playerController.transform);

        // Create States
        CameraState defaultState = new CameraState(stateMachine, CameraStateKey.DEFAULT, this, defaultStateFOVOffset);
        CameraState followTargetState = new CameraState(stateMachine, CameraStateKey.FOLLOW_TARGET, this, followTargetStateFOVOffset);
        CameraState closeUpState = new CameraState(stateMachine, CameraStateKey.CLOSE_UP, this, closeUpStateFOVOffset);

        // Create State Machine
        stateMachine = new StateMachine(new Dictionary<CameraStateKey, FiniteState<CameraStateKey>>()
        {
            { CameraStateKey.DEFAULT, defaultState },
            { CameraStateKey.FOLLOW_TARGET, followTargetState },
            { CameraStateKey.CLOSE_UP, closeUpState }
        }, CameraStateKey.DEFAULT);

    }

    public void Start()
    {
        if (_playerController != null && _playerController.StateMachine != null)
            _playerController.StateMachine.OnStateChanged += (PlayerState state) => OnPlayerStateChange(state);
    }

    public void OnPlayerStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.NULL:
            case PlayerState.IDLE:
                stateMachine.GoToState(CameraStateKey.DEFAULT);
                break;
            case PlayerState.INTERACTION:
                stateMachine.GoToState(CameraStateKey.CLOSE_UP);
                break;
            case PlayerState.WALK:
                stateMachine.GoToState(CameraStateKey.FOLLOW_TARGET);
                break;
        }
    }

    public void OnDestroy()
    {
        if (_playerController != null && _playerController.StateMachine != null)
            _playerController.StateMachine.OnStateChanged -= (PlayerState state) => OnPlayerStateChange(state);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerCameraController))]
public class CustomEditorForScript : Editor
{
    SerializedObject _serializedObject;
    PlayerCameraController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (PlayerCameraController)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif