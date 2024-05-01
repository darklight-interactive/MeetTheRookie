using System.Collections.Generic;
using Darklight.Game.Camera;
using Darklight.Game.Utility;
using UnityEngine;

public class PlayerCameraController : CameraController
{
    PlayerController _playerController => FindFirstObjectByType<PlayerController>();

    public override void Awake()
    {
        // Create States
        CameraState defaultState = new CameraState(CameraStateType.DEFAULT, this, 0f);
        CameraState followTargetState = new CameraState(CameraStateType.FOLLOW_TARGET, this, -0.25f);
        CameraState closeUpState = new CameraState(CameraStateType.CLOSE_UP, this, -0.5f);

        // Create State Machine
        _stateMachine = new StateMachine(new Dictionary<CameraStateType, FiniteState<CameraStateType>>()
        {
            { CameraStateType.DEFAULT, defaultState },
            { CameraStateType.FOLLOW_TARGET, followTargetState },
            { CameraStateType.CLOSE_UP, closeUpState }
        }, CameraStateType.DEFAULT);

    }

    public void Start()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged += (PlayerState state) => OnPlayerStateChange(state);
    }

    public void OnPlayerStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.NONE:
            case PlayerState.IDLE:
                _stateMachine.GoToState(CameraStateType.DEFAULT);
                break;
            case PlayerState.INTERACTION:
                _stateMachine.GoToState(CameraStateType.CLOSE_UP);
                break;
            case PlayerState.WALK:
                _stateMachine.GoToState(CameraStateType.FOLLOW_TARGET);
                break;
        }
    }

    public void OnDestroy()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged -= (PlayerState state) => OnPlayerStateChange(state);
    }
}