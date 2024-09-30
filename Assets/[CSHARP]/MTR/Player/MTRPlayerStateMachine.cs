using System.Collections.Generic;
using Darklight.UnityExt.FMODExt;
using Darklight.UnityExt.Behaviour;
using FMODUnity;
using UnityEngine;

public class MTRPlayerStateMachine : FiniteStateMachine<MTRPlayerState>
{
    protected MTRPlayerController controller;
    protected MTRPlayerInput _input => controller.Input;
    protected PlayerAnimator _animator => controller.Animator;

    /// <param name="args">
    ///    args[0] = PlayerController ( playerController )
    /// </param>
    public MTRPlayerStateMachine(MTRPlayerController controller)
    {
        this.controller = controller;
        possibleStates = new Dictionary<MTRPlayerState, FiniteState<MTRPlayerState>> {
            {MTRPlayerState.NULL, new BasePlayerState(this, MTRPlayerState.NULL)},
            {MTRPlayerState.IDLE, new BasePlayerState(this, MTRPlayerState.IDLE)},
            {MTRPlayerState.WALK, new BasePlayerState(this, MTRPlayerState.WALK)},
            {MTRPlayerState.INTERACTION, new BasePlayerState(this, MTRPlayerState.INTERACTION)},
            {MTRPlayerState.HIDE, new BasePlayerState(this, MTRPlayerState.HIDE)},
            {MTRPlayerState.WALK_OVERRIDE, new WalkOverrideState(this)}};
    }

    public override bool GoToState(MTRPlayerState stateType, bool force = false)
    {
        bool result = base.GoToState(stateType);
        if (result)
        {
            _animator.PlayStateAnimation(stateType);
        }

        return result;
    }


    #region  [[ STATES ]] ======================================================== >>

    public class BasePlayerState : FiniteState<MTRPlayerState>
    {
        protected MTRPlayerStateMachine stateMachine;
        protected MTRPlayerController controller;
        protected MTRPlayerInput input => controller.Input;
        protected PlayerAnimator animator => controller.Animator;
        public BasePlayerState(MTRPlayerStateMachine stateMachine, MTRPlayerState stateType) : base(stateMachine, stateType)
        {
            this.stateMachine = stateMachine;
            this.controller = stateMachine.controller;
        }
        public override void Enter() { }
        public override void Execute() { }
        public override void Exit() { }

    }

    public class WalkOverrideState : BasePlayerState
    {        //private CurrentDestinationPoint _currentDestinationPoint;
        public WalkOverrideState(MTRPlayerStateMachine stateMachine) : base(stateMachine, MTRPlayerState.WALK_OVERRIDE) { }

        public override void Enter()
        {
            if (input.IsInputEnabled == true)
            {
                input.SetInputEnabled(false);
            }
        }

        public override void Execute()
        {
            if (controller.IsAtMoveTarget())
            {
                controller.OverrideResetMoveDirection();
                stateMachine.GoToState(MTRPlayerState.INTERACTION);
            }
        }
    }

    #endregion
}

[System.Serializable]
public class PlayerStateObject : FiniteState<MTRPlayerState>
{
    [Header("Sound Events")]
    public EventReference soundOnEnter;
    public EventReference soundOnExit;
    public EventReference repeatingSound;
    [SerializeField, Range(0.1f, 1f)] private float repeatingSoundInterval = 1f;

    public PlayerStateObject(MTRPlayerStateMachine stateMachine, MTRPlayerState stateType, params object[] args) : base(stateMachine, stateType) { }

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
