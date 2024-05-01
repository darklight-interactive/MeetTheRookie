using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum CameraState
{
    DEFAULT,
    FOLLOW_TARGET,
    CLOSE_UP
}

[ExecuteAlways]
public class CameraController : CameraRig
{

    #region State Machine ============================================== >>>>
    public class StateMachine : FiniteStateMachine<CameraState>
    {
        public StateMachine(Dictionary<CameraState, FiniteState<CameraState>> possibleStates, CameraState initialState, params object[] args) : base(possibleStates, initialState, args) { }
    }

    /// <summary>
    /// This state is the default state for the camera. It does not follow any target and is in a fixed position.
    /// </summary>
    public class State : FiniteState<CameraState>
    {
        private float _FOVOffset;

        /// <param name="args">
        ///     args[0] = CameraRig ( cameraRig )
        ///     args[1] = float ( FOVOffset )
        /// </param>
        public State(CameraState stateType, params object[] args) : base(stateType, args)
        {
            cameraRig = (CameraRig)args[0];
            _FOVOffset = (float)args[1];
        }

        public override void Enter()
        {
            cameraRig.SetFOVOffset(_FOVOffset);
        }

        public override void Exit() { }
        public override void Execute() { }


        protected CameraRig cameraRig;
    }
    #endregion

    [Space(10), Header("Camera Controller")]
    [SerializeField, ShowOnly] PlayerState _currentPlayerState;

    StateMachine _stateMachine;
    PlayerController _playerController => FindFirstObjectByType<PlayerController>();

    public void Awake()
    {
        // Create States
        State defaultState = new State(CameraState.DEFAULT, this, 0f);
        State followTargetState = new State(CameraState.FOLLOW_TARGET, this, -0.25f);
        State closeUpState = new State(CameraState.CLOSE_UP, this, -0.5f);

        // Create State Machine
        _stateMachine = new StateMachine(new Dictionary<CameraState, FiniteState<CameraState>>()
        {
            { CameraState.DEFAULT, defaultState },
            { CameraState.FOLLOW_TARGET, followTargetState },
            { CameraState.CLOSE_UP, closeUpState }
        }, CameraState.DEFAULT);
    }

    public void Start()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged += (PlayerState state) => OnPlayerStateChange(state);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine != null)
            _stateMachine.Step();
    }

    public void OnPlayerStateChange(PlayerState state)
    {
        _currentPlayerState = state;

        switch (state)
        {
            case PlayerState.NONE:
            case PlayerState.IDLE:
                _stateMachine.GoToState(CameraState.DEFAULT);
                break;
            case PlayerState.INTERACTION:
                _stateMachine.GoToState(CameraState.CLOSE_UP);
                break;
            case PlayerState.WALK:
                _stateMachine.GoToState(CameraState.FOLLOW_TARGET);
                break;
        }
    }

    public void OnDestroy()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged -= (PlayerState state) => OnPlayerStateChange(state);
    }
}




