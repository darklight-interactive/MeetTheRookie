using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;
using Darklight.Game.SpriteAnimation;
using System.Linq;
using System;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine = new PlayerStateMachine(PlayerState.IDLE);

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    [Range(0.1f, 5f)] public float playerSpeed = 2.5f;
    public Vector2 moveVector = Vector2.zero;

    [Header("Interactions")]
    public GameObject interactionPopup;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        Invoke("StartInputListener", 1);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate() {
        HandleInteractions();
    }


    #region ===== [ HANDLE MOVE INPUT ] ===== >>
    Vector2 _activeMoveInput = Vector2.zero;
    void StartInputListener()
    {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }

        // Subscribe to Universal MoveInput
        InputAction moveInputAction = UniversalInputManager.MoveInputAction;
        if (moveInputAction == null) { Debug.LogWarning("MoveInputAction is not initialized"); return; }

        moveInputAction.performed += context => _activeMoveInput = moveInputAction.ReadValue<Vector2>();
        moveInputAction.canceled += context => _activeMoveInput = Vector2.zero;
        UniversalInputManager.PrimaryInteractAction.performed += Interact;
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



    #endregion

    #region Interactions
    protected HashSet<InkInteraction> interactions = new HashSet<InkInteraction>();
    public void UnsubscribeInteraction(InkInteraction i) {
        interactions.Remove(i);
    }
    public void SubscribeInteraction(InkInteraction i) {
        interactions.Add(i);
    }

    InkInteraction currentInteraction = null;
    void HandleInteractions() {
        // May want a better priority system, but this is fine for now:
        if (interactions.Count > 0) {
            var toInteract = interactions.First();
            currentInteraction = toInteract;
            ISceneSingleton<UIManager>.Instance.DisplayInteractPrompt(true, toInteract.transform);
        } else if (currentInteraction != null) {
            currentInteraction = null;
            ISceneSingleton<UIManager>.Instance.DisplayInteractPrompt(false);
        }
    }

    void Interact(InputAction.CallbackContext context) {
        if (currentInteraction != null) {
            currentInteraction.Interact();
            currentInteraction = null;
            ISceneSingleton<UIManager>.Instance.DisplayInteractPrompt(false);
        }
    }
    #endregion

}


