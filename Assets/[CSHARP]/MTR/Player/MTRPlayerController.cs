using Darklight.UnityExt.Editor;
using UnityEngine;
using static Darklight.UnityExt.Animation.FrameAnimationPlayer;


public enum MTRPlayerState { NULL, IDLE, WALK, INTERACTION, HIDE, WALKOVERRIDE }
public enum MTRPlayerDirection { NONE, RIGHT, LEFT }

[RequireComponent(typeof(MTRPlayerInput))]
[RequireComponent(typeof(MTRPlayerInteractor))]
[RequireComponent(typeof(PlayerAnimator))]
public class MTRPlayerController : MonoBehaviour
{
    MTRSceneBounds _sceneBounds;
    PlayerStateMachine _stateMachine;

    [SerializeField, ShowOnly] float _speed = 1f;
    [SerializeField, ShowOnly] Vector2 _activeMoveDirection = Vector2.zero;
    [SerializeField] MTRPlayerDirection _direction;

    public MTRPlayerInput Input => GetComponent<MTRPlayerInput>();
    public MTRPlayerInteractor Interactor => GetComponent<MTRPlayerInteractor>();
    public PlayerAnimator Animator => GetComponent<PlayerAnimator>();
    public PlayerStateMachine StateMachine
    {
        get
        {
            if (_stateMachine == null)
                _stateMachine = new PlayerStateMachine(this);
            return _stateMachine;
        }
    }
    public MTRPlayerState CurrentState
    {
        get
        {
            if (StateMachine == null) return MTRPlayerState.NULL;
            return StateMachine.CurrentState;
        }
    }
    public MTRPlayerDirection Facing => _direction;

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
        if (CurrentState == MTRPlayerState.WALKOVERRIDE) return;

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
        if (Facing == MTRPlayerDirection.RIGHT)
            Animator.SetFacing(SpriteDirection.RIGHT);
        else if (Facing == MTRPlayerDirection.LEFT)
            Animator.SetFacing(SpriteDirection.LEFT);

        // Update the State Machine
        if (moveDirection.magnitude > 0.1f)
            StateMachine.GoToState(MTRPlayerState.WALK);
        else
            StateMachine.GoToState(MTRPlayerState.IDLE);
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
    #endregion

    public void HandleMoveInput(Vector2 moveInput) => SetMoveDirection(moveInput);
    public void HandleOnMoveInputCanceled() => SetMoveDirection(Vector2.zero);
    public void OverrideSetMoveDirection(Vector2 moveDirection) => SetMoveDirection(moveDirection);

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

}