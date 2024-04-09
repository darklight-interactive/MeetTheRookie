using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Darklight;
using Darklight.Game.SpriteAnimation;
using Darklight.UnityExt.Input;

using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { NONE, IDLE, WALK, INTERACT }

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine = new PlayerStateMachine(PlayerState.IDLE);
    [Range(0.1f, 5f)] public float playerSpeed = 2.5f;
    public Vector2 moveVector = Vector2.zero; // this is the vector that the player is moving on

    void Start()
    {
        Invoke("StartInputListener", 1);
    }

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

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void FixedUpdate()
    {
        HandleInteractions();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        InkyInteraction interaction = other.GetComponent<InkyInteraction>();
        if (interaction != null)
        {
            SubscribeInteraction(interaction);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        InkyInteraction interaction = other.GetComponent<InkyInteraction>();
        if (interaction != null)
        {
            UnsubscribeInteraction(interaction);
        }
    }

    #region ===== [ MOVEMENT HANDLING ] ===== >>


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

    #region ===== [[ INTERACTION HANDLING ]] ===== >>
    public InkyInteraction targetInteraction;
    public InkyInteraction activeInkInteraction;
    protected HashSet<InkyInteraction> interactions = new HashSet<InkyInteraction>();
    public void UnsubscribeInteraction(InkyInteraction i)
    {
        interactions.Remove(i);
    }
    public void SubscribeInteraction(InkyInteraction i)
    {
        interactions.Add(i);
    }

    protected bool canInteract = true;
    void HandleInteractions()
    {
        if (!canInteract) return;

        if (interactions.Count == 0)
        {
            UXML_InteractionUI.Instance.HideInteractPrompt();
            return;
        }
        else
        {
            // May want a better priority system, but this is fine for now:
            this.targetInteraction = interactions.First();
            this.targetInteraction.DisplayInteractionPrompt(this.targetInteraction.transform.position);
        }
    }

    /// <summary>
    /// Z (or interaction equivalent) has been pressed, pass things off for our interactable to handle.
    /// </summary>
    void Interact(InputAction.CallbackContext context)
    {
        if (targetInteraction != null)
            {
                // Hide Interaction Prompt
                canInteract = false;
            UXML_InteractionUI.Instance.HideInteractPrompt();

            // Transfer the target interaction to the active interaction
            activeInkInteraction = targetInteraction;
            targetInteraction = null;

            // Start the interaction
            activeInkInteraction.StartInteractionKnot(() =>
                {
                    // reset on callback
                    canInteract = true;
                    activeInkInteraction = null;

                });
                Debug.Log("Interact >> Start Ink Interaction");
            }
            else if (activeInkInteraction != null)
            {
            InkyStoryManager.InkDialogue dialogue = InkyStoryManager.Instance.Continue();
            if (dialogue != null)
            {
                Debug.Log("Interact >> Continue Ink Interaction");
            }
                else
                {

                    // End the interaction
                    if (activeInkInteraction)
                        activeInkInteraction.ResetInteraction();
                    activeInkInteraction = null;
                    canInteract = true;
                    Debug.Log("Interact >> End Ink Interaction");
                }
            }
    }
    #endregion

}


