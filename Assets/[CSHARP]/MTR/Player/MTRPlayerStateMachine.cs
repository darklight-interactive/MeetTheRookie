using System.Collections.Generic;
using Darklight.UnityExt.FMODExt;
using Darklight.UnityExt.Behaviour;
using FMODUnity;
using UnityEngine;
using System.Collections;

public class MTRPlayerStateMachine : FiniteStateMachine<MTRPlayerState>
{
    protected MTRPlayerController controller;
    protected MTRPlayerInput _input => controller.Input;
    protected MTRPlayerAnimator _animator => controller.Animator;

    /// <param name="args">
    ///    args[0] = PlayerController ( playerController )
    /// </param>
    public MTRPlayerStateMachine(MTRPlayerController controller)
    {
        this.controller = controller;
        possibleStates = new Dictionary<MTRPlayerState, FiniteState<MTRPlayerState>> {
            {MTRPlayerState.NULL, new BasePlayerState(this, MTRPlayerState.NULL)},

            {MTRPlayerState.IDLE, new BasePlayerState(this, MTRPlayerState.IDLE)},
            {MTRPlayerState.OVERRIDE_IDLE, new BasePlayerState(this, MTRPlayerState.OVERRIDE_IDLE)},

            {MTRPlayerState.WALK, new BasePlayerState(this, MTRPlayerState.WALK)},
            {MTRPlayerState.OVERRIDE_WALK, new WalkOverrideState(this)},

            {MTRPlayerState.INTERACTION, new InteractionState(this)},
            {MTRPlayerState.HIDE, new BasePlayerState(this, MTRPlayerState.HIDE)}
        };
    }

    public override bool GoToState(MTRPlayerState stateType, bool force = false)
    {
        bool result = base.GoToState(stateType);
        if (result)
        {
            SetAnimation(stateType);
            SetInputs(stateType);
        }

        return result;
    }

    void SetAnimation(MTRPlayerState stateType)
    {
        switch (stateType)
        {
            case MTRPlayerState.OVERRIDE_IDLE:
                stateType = MTRPlayerState.IDLE;
                break;
            case MTRPlayerState.OVERRIDE_WALK:
                stateType = MTRPlayerState.WALK;
                break;
        }

        _animator.PlayStateAnimation(stateType);
    }

    void SetInputs(MTRPlayerState stateType)
    {
        switch (stateType)
        {
            case MTRPlayerState.IDLE:
            case MTRPlayerState.WALK:
                _input.SetAllInputsEnabled(true);
                break;
            case MTRPlayerState.INTERACTION:
                _input.SetMovementInputEnabled(false);
                _input.SetInteractInputEnabled(true);
                break;
            default:
                _input.SetAllInputsEnabled(false);
                break;
        }
    }

    #region  [[ STATES ]] ======================================================== >>

    public class BasePlayerState : FiniteState<MTRPlayerState>
    {
        protected MTRPlayerStateMachine stateMachine;
        protected MTRPlayerController controller => stateMachine.controller;
        protected MTRPlayerInput input => controller.Input;
        protected MTRPlayerAnimator animator => controller.Animator;
        protected MTRPlayerInteractor interactor => controller.Interactor;
        public BasePlayerState(MTRPlayerStateMachine stateMachine, MTRPlayerState stateType) : base(stateMachine, stateType)
        {
            this.stateMachine = stateMachine;
        }
        public override void Enter()
        {
            interactor.SetEnabled(true);
        }
        public override void Execute() { }
        public override void Exit() { }

    }

    public class InteractionState : BasePlayerState
    {
        MTRInteractable targetInteractable => interactor.TargetInteractable as MTRInteractable;
        public InteractionState(MTRPlayerStateMachine stateMachine) : base(stateMachine, MTRPlayerState.INTERACTION) { }

        public override void Enter()
        {
            interactor.SetEnabled(false);
        }

        public override void Execute()
        {
            if (targetInteractable != null && targetInteractable.CurrentState == MTRInteractable.State.COMPLETE)
            {
                stateMachine.GoToState(MTRPlayerState.IDLE);
                interactor.ClearTarget();
            }
        }

        public override void Exit()
        {
            interactor.SetEnabled(true);
        }
    }

    public class WalkOverrideState : BasePlayerState
    {
        bool _isAtMoveTarget = false;
        public WalkOverrideState(MTRPlayerStateMachine stateMachine) : base(stateMachine, MTRPlayerState.OVERRIDE_WALK) { }

        public override void Enter()
        {
            _isAtMoveTarget = false;
            if (input.IsAllInputEnabled == true)
            {
                interactor.SetEnabled(false);
            }
        }
        public override void Execute()
        {
            if (controller.IsAtMoveTarget() && !_isAtMoveTarget)
            {
                _isAtMoveTarget = true;

                if (controller == null)
                {
                    Debug.LogError("Controller is null");
                    return;
                }

                controller.OverrideResetMoveDirection();
                controller.StartCoroutine(WaitAndGoToInteractionState());
            }
        }

        public override void Exit()
        {

        }

        IEnumerator WaitAndGoToInteractionState()
        {
            animator.PlayStateAnimation(MTRPlayerState.IDLE); // Play idle animation

            // Wait for the player to face the target position
            Vector3 targetPos = interactor.TargetInteractable.transform.position;
            if (!controller.IsFacingPosition(targetPos))
            {
                yield return new WaitForSeconds(0.15f);
                controller.FacePosition(targetPos);
            }

            // Delay before going to interaction state
            yield return new WaitForSeconds(0.5f);
            stateMachine.GoToState(MTRPlayerState.INTERACTION);
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
