using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
using static Darklight.UnityExt.Animation.FrameAnimationPlayer;


public enum MTRPlayerState { NULL, IDLE, WALK, INTERACTION, HIDE, WALK_OVERRIDE }
public enum MTRPlayerDirection { NONE, RIGHT, LEFT }

[RequireComponent(typeof(MTRPlayerInput))]
[RequireComponent(typeof(MTRPlayerInteractor))]
[RequireComponent(typeof(PlayerAnimator))]
public class MTRPlayerController : MonoBehaviour
{
    MTRSceneBounds _sceneBounds;
    MTRPlayerStateMachine _stateMachine;

    [SerializeField, ShowOnly] MTRPlayerState _currentState = MTRPlayerState.NULL;
    [SerializeField, ShowOnly] float _speed = 1f;
    [SerializeField, ShowOnly] Vector2 _activeMoveDirection = Vector2.zero;
    [SerializeField] MTRPlayerDirection _direction;
    [SerializeField, ShowOnly] float _moveTargetX;

    public MTRPlayerInput Input => GetComponent<MTRPlayerInput>();
    public MTRPlayerInteractor Interactor => GetComponent<MTRPlayerInteractor>();
    public PlayerAnimator Animator => GetComponent<PlayerAnimator>();
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
    public MTRPlayerDirection Facing => _direction;

    void Awake()
    {
        _stateMachine = new MTRPlayerStateMachine(this);
        _stateMachine.OnStateChanged += (state) => _currentState = state;
    }

    void Update()
    {
        StateMachine.Step();
        UpdateMovement();
    }

    #region [ MOVEMENT ] < PRIVATE_METHODS > =============================================== >>
    void UpdateMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (CurrentState == MTRPlayerState.INTERACTION) return;

        // << HANDLE INPUT >>
        Vector2 moveDirection = new Vector2(_activeMoveDirection.x, 0); // Get the horizontal input
        moveDirection *= _speed; // Scalar

        // << SET FACING >>
        if (moveDirection.x > 0) _direction = MTRPlayerDirection.RIGHT;
        if (moveDirection.x < 0) _direction = MTRPlayerDirection.LEFT;

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;

        // Don't allow moving outside of SceneBounds
        if (_sceneBounds)
        {
            if ((transform.position.x > _sceneBounds.leftBound && moveDirection.x < 0) || (transform.position.x < _sceneBounds.rightBound && moveDirection.x > 0))
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }

        // Update the Animator
        SetAnimationDirection(_direction);

        // << UPDATE STATE MACHINE >> ======================================================== >>
        if (Input.IsAllInputEnabled)
        {
            if (moveDirection.magnitude > 0.1f)
                StateMachine.GoToState(MTRPlayerState.WALK);
            else
                StateMachine.GoToState(MTRPlayerState.IDLE);
        }
    }

    void SetMoveDirection(Vector2 moveDirection)
    {
        _activeMoveDirection = moveDirection;
        switch (moveDirection.x)
        {
            case > 0.5f:
                _direction = MTRPlayerDirection.RIGHT;
                break;
            case < -0.5f:
                _direction = MTRPlayerDirection.LEFT;
                break;
            default:
                _direction = MTRPlayerDirection.NONE;
                break;
        }
    }

    void SetAnimationDirection(MTRPlayerDirection direction)
    {
        switch (direction)
        {
            case MTRPlayerDirection.RIGHT:
                Animator.SetFacing(SpriteDirection.RIGHT);
                break;
            case MTRPlayerDirection.LEFT:
                Animator.SetFacing(SpriteDirection.LEFT);
                break;
            case MTRPlayerDirection.NONE:
                break;
        }
    }

    void GetDirectionEnum(Vector2 direction, out MTRPlayerDirection playerDirection)
    {
        if (direction.x > 0)
            playerDirection = MTRPlayerDirection.RIGHT;
        else if (direction.x < 0)
            playerDirection = MTRPlayerDirection.LEFT;
        else
            playerDirection = MTRPlayerDirection.NONE;
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

    void GetDirectionToPos(float targetX, out MTRPlayerDirection direction)
    {
        GetDirectionToPos(targetX, out Vector2 dir);
        GetDirectionEnum(dir, out direction);
    }

    void GetDirectionToInteractable(MTRInteractable interactable, out MTRPlayerDirection direction)
    {
        GetDirectionToPos(interactable.transform.position.x, out Vector2 dir);
        GetDirectionEnum(dir, out direction);
    }
    #endregion

    // << PUBLIC_METHODS >> ==================================================================== >>
    public void HandleMoveInput(Vector2 moveInput) => SetMoveDirection(moveInput);
    public void HandleOnMoveInputCanceled() => SetMoveDirection(Vector2.zero);
    public void OverrideSetMoveDirection(Vector2 moveDirection) => SetMoveDirection(moveDirection);
    public void OverrideResetMoveDirection() => SetMoveDirection(Vector2.zero);
    /// <summary>
    /// Moves the player into the interaction state
    /// </summary>
    public void EnterInteraction()
    {
        if (StateMachine == null) return;
        StateMachine.GoToState(MTRPlayerState.INTERACTION);
        //Debug.Log("Player Controller :: Enter Interaction");
    }

    /// <summary>
    /// Removes the player from the interaction state
    /// </summary>
    public void ExitInteraction()
    {
        StateMachine.GoToState(MTRPlayerState.IDLE);
        //Debug.Log("Player Controller :: Exit Interaction");
    }

    public void StartWalkOverride(float targetXPos)
    {
        if (StateMachine == null) return;

        Debug.Log($"PlayerController :: StartWalkOverride({targetXPos})");

        _moveTargetX = targetXPos;
        GetDirectionToPos(targetXPos, out Vector2 direction);
        OverrideSetMoveDirection(direction);
        StateMachine.GoToState(MTRPlayerState.WALK_OVERRIDE);
    }

    public bool IsAtMoveTarget()
    {
        if (Mathf.Abs(transform.position.x - _moveTargetX) < 0.05f)
        {
            return true;
        }

        if (Facing == MTRPlayerDirection.RIGHT && transform.position.x > _moveTargetX)
        {
            return true;
        }
        else if (Facing == MTRPlayerDirection.LEFT && transform.position.x < _moveTargetX)
        {
            return true;
        }
        return false;
    }

    public bool IsFacingPosition(Vector3 position)
    {
        if (Facing == MTRPlayerDirection.RIGHT && transform.position.x < position.x)
            return true;
        else if (Facing == MTRPlayerDirection.LEFT && transform.position.x > position.x)
            return true;
        return false;
    }

    public void FacePosition(Vector3 position)
    {
        if (transform.position.x < position.x)
            SetAnimationDirection(MTRPlayerDirection.RIGHT);
        else if (transform.position.x > position.x)
            SetAnimationDirection(MTRPlayerDirection.LEFT);
    }
}