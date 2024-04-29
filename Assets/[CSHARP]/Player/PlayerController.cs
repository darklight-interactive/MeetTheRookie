using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Darklight;
using Darklight.Game.SpriteAnimation;
using Darklight.UnityExt.Input;

using UnityEngine;
using UnityEngine.InputSystem;
using static Darklight.UnityExt.CustomInspectorGUI;

public enum PlayerState { NONE, IDLE, WALK, INTERACTION, HIDE }

/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>
[RequireComponent(typeof(PlayerAnimator), typeof(PlayerInteractor))]
public class PlayerController : MonoBehaviour
{
    PlayerInteractor playerInteractor => GetComponent<PlayerInteractor>();
    public PlayerStateMachine stateMachine = new PlayerStateMachine(PlayerState.IDLE);
    [SerializeField, ShowOnly] PlayerState currentState;
    [Range(0.1f, 5f)] public float playerSpeed = 2.5f;
    public Vector2 moveVector = Vector2.zero; // this is the vector that the player is moving on

    Vector2 _activeMoveInput = Vector2.zero;
    void Start()
    {
        Debug.Log($"PlayerController is listening to input from {UniversalInputManager.DeviceInputType}");

        // Subscribe to Universal MoveInput
        InputAction moveInputAction = UniversalInputManager.MoveInputAction;
        if (moveInputAction == null) { Debug.LogWarning("MoveInputAction is not initialized"); return; }

        moveInputAction.performed += context => _activeMoveInput = moveInputAction.ReadValue<Vector2>();
        moveInputAction.canceled += context => _activeMoveInput = Vector2.zero;
        UniversalInputManager.PrimaryInteractAction.performed += Interact;
        UniversalInputManager.SecondaryInteractAction.performed += ToggleSynthesis;
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine.CurrentState != PlayerState.INTERACTION)
        {
            HandleMovement();
        }

        currentState = stateMachine.CurrentState;
    }

    void HandleMovement()
    {
        Vector2 moveDirection = _activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= moveVector; // Nullify the Y axis { Force movement on given axis only }

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

        // Update the Animation
        PlayerAnimator animationManager = GetComponent<PlayerAnimator>();
        if (animationManager == null || animationManager.FrameAnimationPlayer == null) { Debug.Log("Player Controller has no FrameAnimationPlayer"); }
        animationManager.FrameAnimationPlayer.FlipTransform(moveDirection);

        // Update the State Machine
        if (moveDirection != Vector2.zero)
        {
            stateMachine.ChangeState(PlayerState.WALK);
        }
        else
        {
            stateMachine.ChangeState(PlayerState.IDLE);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Get Hidden Object Component
        var hiddenObject = other.GetComponent<Hideable_Object>();
        if (hiddenObject != null)
            {
				// debug.log for proof
                Debug.Log("Character is hidden");
                stateMachine.ChangeState(PlayerState.HIDE);
            }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset state to Walk/Idle 
        if (other.GetComponent<Hideable_Object>() != null)
        {
                stateMachine.ChangeState(PlayerState.IDLE);
            }
    }

    /// <summary>
    /// Interaction Input has been pressed
    /// </summary>
    void Interact(InputAction.CallbackContext context)
    {

        if (playerInteractor.InteractWithActiveTarget())
        {
            stateMachine.ChangeState(PlayerState.INTERACTION);
        }
    }

    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis(InputAction.CallbackContext context) {
        synthesisEnabled = !synthesisEnabled;
        stateMachine.ChangeState(synthesisEnabled ? PlayerState.INTERACTION : PlayerState.IDLE);
        SynthesisManager.Instance.Show(synthesisEnabled);
    }
    #endregion
}


