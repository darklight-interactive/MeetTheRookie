using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;

public enum NPCState { NONE, IDLE, WALK, SPEAK, FOLLOW, HIDE, CHASE }

public class NPC_StateMachine : FiniteStateMachine<NPCState>
{
    private readonly NPC_Animator animator;

    /// <summary>
    /// Constructor for the NPCStateMachine
    /// </summary>
    /// <param name="initialState"> The initial state of the NPC </param>
    /// <param name="possibleStates"> All possible states the NPC can be in </param>
    /// <param name="parent"></param>
    public NPC_StateMachine(NPCState initialState, Dictionary<NPCState, IState<NPCState>> possibleStates, GameObject parent) : base(initialState, possibleStates, parent)
    {
        animator = base.parentObject.GetComponent<NPC_Animator>();
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

        //Debug.Log($"NPC OnStateChanged -> {state}");
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
    private NPC_Animator _animator;

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
        if (_animator == null) { _animator = StateMachine.parentObject.GetComponent<NPC_Animator>(); }
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

        Transform transform = StateMachine.parentObject.transform;
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
    private NPC_Animator _animator;
    private NPC_Controller _controller;

    public SpeakState() { }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parentObject.GetComponent<NPC_Animator>(); }
        if (_controller == null) { _controller = StateMachine.parentObject.GetComponent<NPC_Controller>(); }
    }

    public void Exit() { }

    public void Execute(params object[] executeArgs)
    {
        GameObject player = _controller.player;

        // Set NPC to face player when speaking
        _animator.FrameAnimationPlayer.FlipTransform(new Vector2(StateMachine.parentObject.transform.position.x < player.transform.position.x ? -1 : 1, 0));
    }
}

#endregion

#region ================== [ FOLLOW STATE ] ==================

public class FollowState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private MonoBehaviour _coroutineRunner;
    private NPC_Animator _animator;
    private NPC_Controller _controller;
    private GameObject player;

    private bool movingInFollowState = false;
    private float currentFollowDistance = 0;

    private float followDistance;
    private float followSpeed;

    public FollowState(MonoBehaviour coroutineRunner, ref float followDistance, ref float followSpeed)
    {
        _coroutineRunner = coroutineRunner;
        this.followDistance = followDistance;
        this.followSpeed = followSpeed;

    }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parentObject.GetComponent<NPC_Animator>(); }
        if (_controller == null) { _controller = StateMachine.parentObject.GetComponent<NPC_Controller>(); }
        if (player == null) { player = _controller.player; }

        _coroutineRunner.StartCoroutine(FollowCheck());
    }

    public void Exit()
    {
        _coroutineRunner.StopCoroutine(FollowCheck());
    }

    public void Execute(params object[] executeArgs)
    {
        int followDirection = (currentFollowDistance < 0) ? -1 : 1;
        _animator.FrameAnimationPlayer.FlipTransform(new Vector2(-followDirection, 0));

        if (movingInFollowState)
        {
            GameObject npc = StateMachine.parentObject;

            float movement = followDirection * followSpeed;
            float targetX = npc.transform.position.x + movement;

            // move the character
            npc.transform.position = Vector3.Lerp(npc.transform.position, new Vector3(targetX, npc.transform.position.y, npc.transform.position.z), Time.deltaTime);
        }
    }

    private IEnumerator FollowCheck()
    {
        for (; ; )
        {
            // check 20 times per second
            yield return new WaitForSeconds(1 / 20);

            // If NPC is moving and they're within distance, stop
            // If NPC not moving and they're outside distance, start
            currentFollowDistance = player.transform.position.x - StateMachine.parentObject.transform.position.x;
            bool beyondFollowDistance = Mathf.Abs(currentFollowDistance) > followDistance;
            if (!movingInFollowState && beyondFollowDistance)
            {
                movingInFollowState = true;
                _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(NPCState.WALK));
            }
            else if (movingInFollowState && !beyondFollowDistance)
            {
                movingInFollowState = false;
                _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(NPCState.IDLE));
            }
        }
    }
}

#endregion

#region ================== [ HIDE STATE ] ==================

public class HideState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private NPC_Animator _animator;
    private NPC_Controller _controller;
    private MonoBehaviour _coroutineRunner;

    private Hideable_Object[] hideableObjects;
    private GameObject closestHideableObject;
    private bool areThereHideableObjects = false;
    private bool movingInHideState = false;
    private readonly float validHideDistance = 0.01f;

    private float hideSpeed;

    public HideState(MonoBehaviour coroutineRunner, ref float hideSpeed)
    {
        _coroutineRunner = coroutineRunner;
        this.hideSpeed = hideSpeed;
    }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parentObject.GetComponent<NPC_Animator>(); }
        if (_controller == null) { _controller = StateMachine.parentObject.GetComponent<NPC_Controller>(); }

        _coroutineRunner.StartCoroutine(HideCheck());
    }

    public void Exit()
    {
        _controller.StopCoroutine(HideCheck());
    }

    public void Execute(params object[] executeArgs)
    {
        if (movingInHideState)
        {
            GameObject npc = _controller.gameObject;

            float currentHideDistance = closestHideableObject.transform.position.x - npc.transform.position.x;
            int hideDirection = (currentHideDistance < 0) ? -1 : 1;

            float movement = hideDirection * hideSpeed;
            float targetX = npc.transform.position.x + movement;

            // move the character
            npc.transform.position = Vector3.Lerp(npc.transform.position,
                new Vector3(targetX, npc.transform.position.y, npc.transform.position.z),
                Time.deltaTime);
            _animator.FrameAnimationPlayer.FlipTransform(new Vector2(-hideDirection, 0));
        }
    }

    private IEnumerator HideCheck()
    {
        int hideCheckCount = 0;
        for (; ; )
        {
            hideableObjects = GameObject.FindObjectsOfType<Hideable_Object>();

            // find the closest hideable object, only if there are any
            if (hideableObjects != null && hideableObjects.Length > 0)
            {
                areThereHideableObjects = true;

                GameObject currentClosest = hideableObjects[0].gameObject;
                float currentPos = StateMachine.parentObject.transform.position.x;
                float currentClosestDistance = Mathf.Abs(currentPos - currentClosest.transform.position.x);

                for (int i = 1; i < hideableObjects.Length; i++)
                {
                    float newDistance = Mathf.Abs(currentPos - hideableObjects[i].transform.position.x);

                    if (newDistance < currentClosestDistance)
                    {
                        currentClosest = hideableObjects[i].gameObject;
                        currentClosestDistance = newDistance;
                    }
                }

                // set the closest hideableObject
                closestHideableObject = currentClosest;

                // Setup animation states
                bool beyondValidHideDistance = currentClosestDistance > validHideDistance;
                // we are out of range, need to start moving
                if (beyondValidHideDistance && !movingInHideState)
                {
                    movingInHideState = true;
                    _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(NPCState.WALK));
                }
                // We just got in range, and need to stop moving
                else if (!beyondValidHideDistance && movingInHideState)
                {
                    movingInHideState = false;
                    _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(NPCState.HIDE));
                }
            }
            else
            // if there are no hideable objects, then we need to just play the idle anim
            // but only if there used to be hideable objects or this is the first check
            {
                if (areThereHideableObjects || hideCheckCount == 0)
                {
                    _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(NPCState.IDLE));
                    areThereHideableObjects = false;
                    movingInHideState = false;
                }
            }

            // time between checks
            yield return new WaitForSeconds(1 / 20);
            hideCheckCount++;
        }
    }
}

#endregion

#region ================== [ CHASE STATE ] ==================

public class ChaseState : IState<NPCState>
{
    public FiniteStateMachine<NPCState> StateMachine { get; set; }
    private NPC_Animator _animator;
    private NPC_Controller _controller;
    private GameObject player;

    private float chaseSpeakDistance;
    private float chaseSpeed;

    public ChaseState(ref float chaseSpeakDistance, ref float chaseSpeed)
    {
        this.chaseSpeakDistance = chaseSpeakDistance;
        this.chaseSpeed = chaseSpeed;
    }

    public void Enter(params object[] enterArgs)
    {
        if (_animator == null) { _animator = StateMachine.parentObject.GetComponent<NPC_Animator>(); }
        if (_controller == null) { _controller = StateMachine.parentObject.GetComponent<NPC_Controller>(); }
        if (player == null) { player = _controller.player; }
    }

    public void Exit() { }

    public void Execute(params object[] executeArgs)
    {
        GameObject npc = StateMachine.parentObject.gameObject;

        float currentChaseDistance = player.transform.position.x - npc.transform.position.x;

        if (currentChaseDistance <= chaseSpeakDistance)
        {
            StateMachine.GoToState(NPCState.SPEAK);
            return;
        }

        int chaseDirection = (currentChaseDistance < 0) ? -1 : 1;

        float movement = chaseDirection * chaseSpeed;
        float targetX = npc.transform.position.x + movement;

        // move the character
        npc.transform.position = Vector3.Lerp(npc.transform.position, new Vector3(targetX, npc.transform.position.y, npc.transform.position.z), Time.deltaTime);
        _animator.FrameAnimationPlayer.FlipTransform(new Vector2(-chaseDirection, 0));
    }
}

#endregion