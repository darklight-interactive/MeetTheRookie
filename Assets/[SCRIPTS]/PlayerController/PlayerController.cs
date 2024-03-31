using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;

public enum PlayerState { IDLE, WALK, INTERACT }
public class PlayerStateMachine : StateMachine<PlayerState>
{
    public PlayerStateMachine(PlayerState state) : base(state) { }

    public new void OnStateChanged(PlayerState previousState, PlayerState newState)
    {
        base.OnStateChanged(previousState, newState);


    }
}

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine = new PlayerStateMachine(PlayerState.IDLE);
    public PlayerState currentState;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    [Range(0.1f, 5f)] public float playerSpeed = 2.5f;


    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        Invoke("StartInputListener", 1);


    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        currentState = StateMachine.CurrentState;
    }


    #region ===== [ HANDLE MOVE INPUT ] ===== >>
    Vector2 _activeMoveInput = Vector2.zero;
    void StartInputListener()
    {
        // Subscribe to Universal MoveInput
        InputAction moveInputAction = UniversalInputManager.MoveInputAction;
        moveInputAction.performed += context => _activeMoveInput = moveInputAction.ReadValue<Vector2>();
        moveInputAction.canceled += context => _activeMoveInput = Vector2.zero;
    }

    void HandleMovement()
    {
        Vector2 moveDirection = _activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= Vector2.right; // Nullify the Y axis

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);


        // Update the Animation
        PlayerAnimator animationManager = GetComponent<PlayerAnimator>();
        animationManager.FrameAnimationPlayer.FlipTransform(moveDirection);
    }
    #endregion


}


