using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game;
using UnityEngine;

public enum NPCState { NONE, IDLE, WALK, SPEAK, FOLLOW, HIDE, CHASE }

public class NPCStateMachine : FiniteStateMachine<NPCState>
{
    private readonly NPCAnimator animator;

    // Constructor
    public NPCStateMachine(NPCState initialState, Dictionary<NPCState, IState<NPCState>> possibleStates, GameObject NPC) : base(initialState, possibleStates, NPC)
    {
        animator = parent.GetComponent<NPCAnimator>();
    }

    public override void Step()
    {
        base.Step();
    }

    public override void GoToState(NPCState state, params object[] enterArgs)
    {
        base.GoToState(state, enterArgs);

        // Load the related Spritesheet to the FrameAnimationPlayer
        if (animator == null) return;
        if (currentState == NPCState.NONE) return;

        //Debug.Log($"NPC OnStateChanged {previousState} -> {newState}");
        animator.FrameAnimationPlayer.LoadSpriteSheet(animator.GetSpriteSheetWithState(currentState));
    }
}

#region ================== [ IDLE STATE ] ==================

// IDLE
public class IdleState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private readonly float _maxDuration;
    private readonly MonoBehaviour _coroutineRunner;

    public IdleState(MonoBehaviour coroutineRunner, ref float idleMaxDuration)
    {
        _coroutineRunner = coroutineRunner;
        _maxDuration = idleMaxDuration;
    }

    public void Enter(params object[] enterArgs)
    {
        _coroutineRunner.StartCoroutine(IdleTimer());
    }

    public void Exit()
    {
        _coroutineRunner.StopCoroutine(IdleTimer());
    }

    public void Execute(params object[] executeArgs) { }

    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, _maxDuration));
        StateMachine.GoToState(NPCState.WALK);
    }
}

#endregion

#region ================== [ WALK STATE ] ==================

// WALK
public class WalkState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private NPCAnimator _animator;
    
    private readonly float _maxDuration;
    private int _walkDirection = 1;
    private readonly float _leftBound;
    private readonly float _rightBound;
    private readonly float _walkSpeed;

    private readonly MonoBehaviour _coroutineRunner;

    public WalkState(MonoBehaviour coroutineRunner, ref float walkSpeed, ref float walkMaxDuration, ref float leftBound, ref float rightBound)
    {
        _coroutineRunner = coroutineRunner;
        _walkSpeed = walkSpeed;
        _maxDuration = walkMaxDuration;
        _leftBound = leftBound;
        _rightBound = rightBound;
    }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parent.GetComponent<NPCAnimator>(); }
        if (_maxDuration == 0) { StateMachine.GoToState(NPCState.IDLE); }

        // When walking, it can be either direction randomly
        _walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;
        _coroutineRunner.StartCoroutine(WalkTimer());
    }

    public void Exit()
    {
        _coroutineRunner.StopCoroutine(WalkTimer());
    }

    public void Execute(params object[] executeArgs)
    {

        Transform transform = StateMachine.parent.transform;
        float movement = _walkDirection * _walkSpeed;
        float targetX = transform.position.x + movement;

        // If we're going out of bounds, flip directions
        if (targetX < _leftBound || targetX > _rightBound)
        {
            _walkDirection *= -1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }

        // If we are already out of bounds, we want to walk back in bounds
        if (transform.position.x <= _leftBound)
        {
            _walkDirection = 1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }
        if (transform.position.x >= _rightBound)
        {
            _walkDirection = -1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }

        // move the character
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

        // Update the Animation
        _animator.FrameAnimationPlayer.FlipTransform(new Vector2(-_walkDirection, 0));
    }

    private IEnumerator WalkTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, _maxDuration));
        StateMachine.GoToState(NPCState.IDLE);
    }
}

#endregion

#region ================== [ SPEAK STATE ] ==================

public class SpeakState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private NPCAnimator _animator;
    private NPCController _controller;

    public SpeakState() { }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parent.GetComponent<NPCAnimator>(); }
        if (_controller == null) { _controller = StateMachine.parent.GetComponent<NPCController>(); }
    }

    public void Exit() { }

    public void Execute(params object[] executeArgs)
    {
        GameObject player = _controller.player;

        // Set NPC to face player when speaking
        _animator.FrameAnimationPlayer.FlipTransform(new Vector2(StateMachine.parent.transform.position.x < player.transform.position.x ? -1 : 1, 0));
    }
}

#endregion