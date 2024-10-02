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
            {MTRPlayerState.INTERACTION, new InteractionState(this)},
            {MTRPlayerState.HIDE, new BasePlayerState(this, MTRPlayerState.HIDE)},
            {MTRPlayerState.WALK_OVERRIDE, new WalkOverrideState(this)}};
    }

    public override bool GoToState(MTRPlayerState stateType, bool force = false)
    {
        bool result = base.GoToState(stateType);
        if (result)
        {
            if (stateType == MTRPlayerState.WALK_OVERRIDE)
                stateType = MTRPlayerState.WALK;
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
        protected MTRPlayerInteractor interactor => controller.Interactor;
        public BasePlayerState(MTRPlayerStateMachine stateMachine, MTRPlayerState stateType) : base(stateMachine, stateType)
        {
            this.stateMachine = stateMachine;
            this.controller = stateMachine.controller;
        }
        public override void Enter() { }
        public override void Execute() { }
        public override void Exit() { }

    }

    public class InteractionState : BasePlayerState
    {
        MTRInteractable targetInteractable => interactor.TargetInteractable as MTRInteractable;
        public InteractionState(MTRPlayerStateMachine stateMachine) : base(stateMachine, MTRPlayerState.INTERACTION) { }

        public override void Enter()
        {
            input.SetMovementInputEnabled(false);
            input.SetInteractInputEnabled(true);
            interactor.SetEnabled(false);
        }

        public override void Execute()
        {
            if (targetInteractable.CurrentState == MTRInteractable.State.COMPLETE)
            {
                input.SetAllInputsEnabled(true);
                stateMachine.GoToState(MTRPlayerState.IDLE);
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
        public WalkOverrideState(MTRPlayerStateMachine stateMachine) : base(stateMachine, MTRPlayerState.WALK_OVERRIDE) { }

        public override void Enter()
        {
            _isAtMoveTarget = false;
            if (input.IsAllInputEnabled == true)
            {
                input.SetAllInputsEnabled(false);
                interactor.SetEnabled(false);
            }
        }

        public override void Execute()
        {
            if (controller.IsAtMoveTarget() && !_isAtMoveTarget)
            {
                _isAtMoveTarget = true;
                controller.OverrideResetMoveDirection();
                controller.StartCoroutine(WaitAndGoToInteractionState());
            }
        }

        public override void Exit()
        {

        }

        IEnumerator WaitAndGoToInteractionState()
        {
            Vector3 targetPos = interactor.TargetInteractable.transform.position;
            stateMachine.GoToState(MTRPlayerState.IDLE); // Go to idle state for animation

            // Wait for the player to face the target position
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
