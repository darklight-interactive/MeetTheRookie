using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
using static Darklight.UnityExt.Animation.FrameAnimationPlayer;


public enum MTRPlayerState
{
    NULL,
    FREE_IDLE,
    FREE_WALK,
    INTERACTION,
    HIDE,
    OVERRIDE_WALK,
    OVERRIDE_IDLE
}
public enum MTRPlayerDirectionFacing { NONE, RIGHT, LEFT }

[RequireComponent(typeof(MTRPlayerInput))]
[RequireComponent(typeof(MTRPlayerInteractor))]
[RequireComponent(typeof(MTRPlayerAnimator))]
public class MTRPlayerController : MonoBehaviour
{
    readonly float _speed = 1f;
    readonly float _boundPadding = 0.05f;
    MTRPlayerStateMachine _stateMachine;


    [Header("States")]
    [SerializeField, ShowOnly] MTRPlayerState _currentState = MTRPlayerState.NULL;
    [SerializeField] MTRPlayerDirectionFacing _directionFacing;

    [Header("Movement")]
    [SerializeField, ShowOnly] float _moveDir;
    [SerializeField, ShowOnly] float _currentPosX;
    [SerializeField, ShowOnly] float _overrideTargetPosX;
    [SerializeField, ShowOnly] bool _inBounds;
    [SerializeField, ShowOnly] bool _canWalkLeft;
    [SerializeField, ShowOnly] bool _canWalkRight;

    public MTRPlayerInput Input => GetComponent<MTRPlayerInput>();
    public MTRPlayerInteractor Interactor => GetComponent<MTRPlayerInteractor>();
    public MTRPlayerAnimator Animator => GetComponent<MTRPlayerAnimator>();
    public MTRPlayerStateMachine StateMachine
    {
        get
        {
            if (_stateMachine == null)
                _stateMachine = new MTRPlayerStateMachine(this);
            return _stateMachine;
        }
    }
    public MTRPlayerState CurrentState
    {
        get
        {
            if (_stateMachine == null)
                _currentState = MTRPlayerState.NULL;
            else
                _currentState = _stateMachine.CurrentState;
            return _currentState;
        }
    }
    public MTRPlayerDirectionFacing DirectionFacing => _directionFacing;

    void Awake()
    {
        _stateMachine = new MTRPlayerStateMachine(this);
        _stateMachine.OnStateChanged += (state) => _currentState = state;
    }

    void FixedUpdate()
    {
        StateMachine.Step();
        UpdateMovement();
    }

    #region [ MOVEMENT ] < PRIVATE_METHODS > =============================================== >>
    void UpdateMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (CurrentState == MTRPlayerState.INTERACTION) return;

        // << Calculate Direction >>
        if (_moveDir < 0) _directionFacing = MTRPlayerDirectionFacing.LEFT;
        else if (_moveDir > 0) _directionFacing = MTRPlayerDirectionFacing.RIGHT;

        // << UPDATE ANIMATOR >>
        SetAnimationDirection(_directionFacing);

        // << CALCULATE POSITION >>
        _currentPosX = transform.position.x;

        // << CALCULATE RELATION TO BOUNDS >>
        MTRSceneBounds sceneBounds = MTRSceneManager.ActiveSceneData.SceneBounds;
        _inBounds = sceneBounds.Contains(_currentPosX);
        _canWalkLeft = _currentPosX > sceneBounds.Left + (_boundPadding * 2);
        _canWalkRight = _currentPosX < sceneBounds.Right - (_boundPadding * 2);



        // << CALCULATE TARGET POSITION >>
        // Default target position is the current position + the move direction * speed
        Vector3 targetPos = new Vector3(_currentPosX + _moveDir, transform.position.y, transform.position.z);
        if (CurrentState == MTRPlayerState.OVERRIDE_WALK)
        {
            // If the player is in an override walk state, the target position is the override target position
            targetPos = new Vector3(_overrideTargetPosX, transform.position.y, transform.position.z);
        }


        // << UPDATE POSITION >>
        bool isWalkingLeft = _canWalkLeft && _moveDir < 0;
        bool isWalkingRight = _canWalkRight && _moveDir > 0;
        if (_inBounds && (isWalkingLeft || isWalkingRight))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        }
        else
        {
            // Clamp the player's position to the scene bounds
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, sceneBounds.Left + _boundPadding, sceneBounds.Right - _boundPadding);
            transform.position = clampedPos;
        }

        // << RETURN IF IN OVERRIDE STATE >>
        if (CurrentState == MTRPlayerState.OVERRIDE_IDLE) return;
        if (CurrentState == MTRPlayerState.OVERRIDE_WALK) return;

        // << UPDATE FREE STATE >>
        if (Mathf.Abs(_moveDir) < 0.1f)
            StateMachine.GoToState(MTRPlayerState.FREE_IDLE);
        else
            StateMachine.GoToState(MTRPlayerState.FREE_WALK);
    }

    void SetMoveDirection(float moveDir)
    {
        _moveDir = Mathf.Clamp(moveDir, -1, 1);
        switch (moveDir)
        {
            case > 0.5f:
                _directionFacing = MTRPlayerDirectionFacing.RIGHT;
                break;
            case < -0.5f:
                _directionFacing = MTRPlayerDirectionFacing.LEFT;
                break;
            default:
                _directionFacing = MTRPlayerDirectionFacing.NONE;
                break;
        }
    }

    void SetAnimationDirection(MTRPlayerDirectionFacing direction)
    {
        switch (direction)
        {
            case MTRPlayerDirectionFacing.RIGHT:
                Animator.SetFacing(SpriteDirection.RIGHT);
                break;
            case MTRPlayerDirectionFacing.LEFT:
                Animator.SetFacing(SpriteDirection.LEFT);
                break;
            case MTRPlayerDirectionFacing.NONE:
                break;
        }
    }

    void GetDirectionEnum(Vector2 direction, out MTRPlayerDirectionFacing playerDirection)
    {
        if (direction.x > 0)
            playerDirection = MTRPlayerDirectionFacing.RIGHT;
        else if (direction.x < 0)
            playerDirection = MTRPlayerDirectionFacing.LEFT;
        else
            playerDirection = MTRPlayerDirectionFacing.NONE;
    }

    void GetDirectionToPos(float targetX, out Vector2 direction)
    {
        float distance = Mathf.Abs(targetX - transform.position.x);
        if (distance < 0.01f)
        {
            direction = Vector2.zero;
            return;
        }
        else
        {
            direction = targetX > transform.position.x ? Vector2.right : Vector2.left;
        }
    }

    void GetDirectionToPos(float targetX, out MTRPlayerDirectionFacing direction)
    {
        GetDirectionToPos(targetX, out Vector2 dir);
        GetDirectionEnum(dir, out direction);
    }

    void GetDirectionToInteractable(MTRInteractable interactable, out MTRPlayerDirectionFacing direction)
    {
        GetDirectionToPos(interactable.transform.position.x, out Vector2 dir);
        GetDirectionEnum(dir, out direction);
    }
    #endregion

    // << PUBLIC_METHODS >> ==================================================================== >>
    public void HandleMoveInput(Vector2 moveInput)
    {
        Debug.Log("<MTRPlayerController> HandleMoveInput :: " + moveInput);
        SetMoveDirection(moveInput.x);
    }
    public void HandleOnMoveInputCanceled() => SetMoveDirection(0);
    public void OverrideSetMoveDirection(Vector2 moveDirection) => SetMoveDirection(moveDirection.x);
    public void OverrideResetMoveDirection()
    {
        _overrideTargetPosX = _currentPosX;
        SetMoveDirection(0);
        StateMachine.GoToState(MTRPlayerState.OVERRIDE_IDLE);
    }
    /// <summary>
    /// Moves the player into the interaction state
    /// </summary>
    public void EnterInteraction()
    {
        StateMachine.GoToState(MTRPlayerState.INTERACTION);
        //Debug.Log("Player Controller :: Enter Interaction");
    }

    /// <summary>
    /// Removes the player from the interaction state
    /// </summary>
    public void ExitInteraction()
    {
        StateMachine.GoToState(MTRPlayerState.FREE_IDLE);
        //Debug.Log("Player Controller :: Exit Interaction");
    }

    public void StartWalkOverride(float targetXPos)
    {
        if (StateMachine == null) return;

        Debug.Log($"PlayerController :: StartWalkOverride({targetXPos})");

        _overrideTargetPosX = targetXPos;
        GetDirectionToPos(targetXPos, out Vector2 direction);

        OverrideSetMoveDirection(direction);
        StateMachine.GoToState(MTRPlayerState.OVERRIDE_WALK);
    }

    public bool IsAtMoveTarget()
    {
        if (Mathf.Abs(transform.position.x - _overrideTargetPosX) < 0.05f)
        {
            return true;
        }

        if (DirectionFacing == MTRPlayerDirectionFacing.RIGHT && transform.position.x > _overrideTargetPosX)
        {
            return true;
        }
        else if (DirectionFacing == MTRPlayerDirectionFacing.LEFT && transform.position.x < _overrideTargetPosX)
        {
            return true;
        }
        return false;
    }

    public bool IsFacingPosition(Vector3 position)
    {
        if (DirectionFacing == MTRPlayerDirectionFacing.RIGHT && transform.position.x < position.x)
            return true;
        else if (DirectionFacing == MTRPlayerDirectionFacing.LEFT && transform.position.x > position.x)
            return true;
        return false;
    }

    public void FacePosition(Vector3 position)
    {
        if (transform.position.x < position.x)
            SetAnimationDirection(MTRPlayerDirectionFacing.RIGHT);
        else if (transform.position.x > position.x)
            SetAnimationDirection(MTRPlayerDirectionFacing.LEFT);
    }
}