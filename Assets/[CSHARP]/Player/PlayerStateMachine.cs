using System.Collections.Generic;
using Darklight.UnityExt.FMODExt;
using Darklight.UnityExt.Behaviour;
using FMODUnity;
using UnityEngine;

public class PlayerStateMachine : FiniteStateMachine<PlayerState>
{
    public PlayerController _controller;
    public PlayerAnimator _animator => _controller.animator;

    /// <param name="args">
    ///    args[0] = PlayerController ( playerController )
    /// </param>
    public PlayerStateMachine(Dictionary<PlayerState, FiniteState<PlayerState>> possibleStates, PlayerState initialState, params object[] args) : base(possibleStates, initialState, args)
    {
        _controller = (PlayerController)args[0];
    }

    public override void Step()
    {
        base.Step();
    }

    public override bool GoToState(PlayerState stateType)
    {
        bool result = base.GoToState(stateType);
        if (result)
        {
            _animator.PlayStateAnimation(stateType);
        }

        return result;
    }
}

[System.Serializable]
public class PlayerStateObject : FiniteState<PlayerState>
{
    [Header("Sound Events")]
    public EventReference soundOnEnter;
    public EventReference soundOnExit;
    public EventReference repeatingSound;
    [SerializeField, Range(0.1f, 1f)] private float repeatingSoundInterval = 1f;

    public PlayerStateObject(PlayerStateMachine stateMachine, PlayerState stateType, params object[] args) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        // Debug.Log($"Entering State: {stateType}");
        FMODExt_EventManager.PlayOneShot(soundOnEnter);
        FMODExt_EventManager.Instance.StartRepeatingEvent(repeatingSound, repeatingSoundInterval);
    }

    public override void Exit()
    {
        // Debug.Log($"Exiting State: {stateType}");
        FMODExt_EventManager.PlayOneShot(soundOnExit);
        FMODExt_EventManager.Instance.StopRepeatingEvent();
    }

    public override void Execute() { }
}
