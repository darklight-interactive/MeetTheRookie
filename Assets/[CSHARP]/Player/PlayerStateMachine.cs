using System.Collections.Generic;
using Darklight.UnityExt.FMODExt;
using Darklight.UnityExt.Behaviour;
using FMODUnity;
using UnityEngine;

public class PlayerStateMachine : FiniteStateMachine<PlayerState>
{
    public MTRPlayerInputController _controller;
    public PlayerAnimator _animator => _controller.Animator;

    /// <param name="args">
    ///    args[0] = PlayerController ( playerController )
    /// </param>
    public PlayerStateMachine(MTRPlayerInputController controller)
    {
        _controller = controller;
        possibleStates = new Dictionary<PlayerState, FiniteState<PlayerState>> {
            {PlayerState.NULL, new FinitePlayerState(this, PlayerState.NULL)},
            {PlayerState.IDLE, new FinitePlayerState(this, PlayerState.IDLE)},
            {PlayerState.WALK, new FinitePlayerState(this, PlayerState.WALK)},
            {PlayerState.INTERACTION, new FinitePlayerState(this, PlayerState.INTERACTION)},
            {PlayerState.HIDE, new FinitePlayerState(this, PlayerState.HIDE)},
            {PlayerState.WALKOVERRIDE, new WalkOverride(this, PlayerState.WALKOVERRIDE)}};
    }

    public override bool GoToState(PlayerState stateType, bool force = false)
    {
        bool result = base.GoToState(stateType);
        if (result)
        {
            _animator.PlayStateAnimation(stateType);
        }

        return result;
    }


    #region  [[ STATE MACHINE ]] ======================================================== >>

    public class FinitePlayerState : FiniteState<PlayerState>
    {
        /// <param name="args">
        ///   args[0] = PlayerController ( playerController )
        public FinitePlayerState(PlayerStateMachine stateMachine, PlayerState stateType) : base(stateMachine, stateType) { }

        public override void Enter()
        {
            // Debug.Log($"Entering State: {stateType}");
        }

        public override void Exit()
        {
            // Debug.Log($"Exiting State: {stateType}");
        }

        public override void Execute()
        {
            // Debug.Log($"Executing State: {stateType}");
        }
    }

    public class WalkOverride : FinitePlayerState
    {
        public PlayerStateMachine _stateMachine;
        private float _walkDestinationX;
        //private CurrentDestinationPoint _currentDestinationPoint;

        public WalkOverride(PlayerStateMachine stateMachine, PlayerState stateType) : base(stateMachine, stateType)
        {
            _stateMachine = stateMachine;
            //_currentDestinationPoint = destination;
        }

        public override void Enter()
        {
            // Debug.Log($"Entering State: WALK");
        }

        public override void Exit()
        {
            // Debug.Log($"Exiting State: {stateType}");
        }

        public override void Execute()
        {
            float _walkDirection = 0;
            float _walkSpeed = 1;
            Transform transform = _stateMachine._controller.transform;
            //_walkDestinationX = _currentDestinationPoint.destinationPoint.x;

            if (Mathf.Abs(transform.position.x - _walkDestinationX) < .1)
            {
                _stateMachine.GoToState(PlayerState.IDLE);
                return;
            }

            if (transform.position.x > _walkDestinationX)
            {
                _walkDirection = -1;
            }
            else
            {
                _walkDirection = 1;
            }

            float movement = _walkDirection * _walkSpeed;
            float targetX = transform.position.x + movement;

            // move the character
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

            // Update the Animation
            //_stateMachine._animator.FrameAnimationPlayer.FlipSprite(new Vector2(_walkDirection, 0));
        }

    }

    #endregion
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
