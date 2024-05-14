using System;
using System.Collections.Generic;
using Darklight.UnityExt.Input;

using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Utility;

/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>

public class PlayerController : MonoBehaviour
{
    public PlayerInteractor interactor => GetComponentInChildren<PlayerInteractor>();
    public PlayerAnimator animator => GetComponentInChildren<PlayerAnimator>();
    public PlayerUIGrid dialogueHandler => GetComponentInChildren<PlayerUIGrid>();
    public PlayerCameraController cameraController => FindFirstObjectByType<PlayerCameraController>();
    public PlayerStateMachine stateMachine { get; private set; }

    [SerializeField, ShowOnly] PlayerStateType _currentState = PlayerStateType.NONE;
    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;


    [Header("Settings")]
    private float playerSpeed = 1f;
    private Vector2 posXVector = Vector2.right; // this is the vector that the player is moving on
    private SceneBounds sceneBounds;

    [Header("States")]
    public PlayerStateObject idleState;
    public PlayerStateObject walkState;
    public PlayerStateObject interactionState;
    public PlayerStateObject hideState;

    void Awake()
    {
        // >> CREATE THE STATE MACHINE
        stateMachine = new PlayerStateMachine(new Dictionary<PlayerStateType, FiniteState<PlayerStateType>> {
            { PlayerStateType.NONE, new PlayerStateObject(PlayerStateType.NONE, this) },
            { PlayerStateType.IDLE, idleState },
            { PlayerStateType.WALK, walkState },
            { PlayerStateType.INTERACTION, interactionState },
            { PlayerStateType.HIDE, hideState }
        }, PlayerStateType.IDLE, this);

        // >> SET UP STATE MACHINE LISTENERS
        stateMachine.OnStateChanged += (PlayerStateType state) => _currentState = state;
    }

    void Start()
    {
        UniversalInputManager.OnMoveInput += (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputStarted += (Vector2 input) =>
        {
            _activeMoveInput = input;
            stateMachine.GoToState(PlayerStateType.WALK);
        };
        UniversalInputManager.OnMoveInputCanceled += () =>
        {
            _activeMoveInput = Vector2.zero;
            stateMachine.GoToState(PlayerStateType.IDLE);
        };
        UniversalInputManager.OnPrimaryInteract += Interact;
        UniversalInputManager.OnSecondaryInteract += ToggleSynthesis;


        // << Find SceneBounds >>
        SceneBounds[] bounds = FindObjectsByType<SceneBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (bounds.Length > 0)
        {
            sceneBounds = bounds[0];
        }
        else
        {
            sceneBounds = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void OnDestroy()
    {
        stateMachine.OnStateChanged -= (PlayerStateType state) => _currentState = state;

        UniversalInputManager.OnMoveInput -= (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputCanceled -= () => _activeMoveInput = Vector2.zero;
        UniversalInputManager.OnPrimaryInteract -= Interact;
        UniversalInputManager.OnSecondaryInteract -= ToggleSynthesis;
    }

    void HandleMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (stateMachine.CurrentState == PlayerStateType.INTERACTION) return;

        Vector2 moveDirection = _activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= posXVector; // Nullify the Y axis { Force movement on given axis only }

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;

        // Don't allow moving outside of SceneBounds
        if (sceneBounds)
        {
            if ((transform.position.x > sceneBounds.leftBound && moveDirection.x < 0) || (transform.position.x < sceneBounds.rightBound && moveDirection.x > 0))
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }

        // Update the Animation
        if (animator == null || animator.FrameAnimationPlayer == null) { Debug.Log("Player Controller has no FrameAnimationPlayer"); }
        animator.FrameAnimationPlayer.FlipTransform(moveDirection);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Get Hidden Object Component
        Hideable_Object hiddenObject = other.GetComponent<Hideable_Object>();
        if (hiddenObject != null)
        {
            // debug.log for proof
            Debug.Log("Character is hidden");
            stateMachine.GoToState(PlayerStateType.HIDE);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset state to Walk/Idle 
        if (other.GetComponent<Hideable_Object>() != null)
        {
            stateMachine.GoToState(PlayerStateType.IDLE);
        }
    }

    /// <summary>
    /// Interaction Input has been pressed
    /// </summary>
    void Interact()
    {
        if (stateMachine.CurrentState != PlayerStateType.INTERACTION)
        {
            stateMachine.GoToState(PlayerStateType.INTERACTION);
        }
        interactor.InteractWithTarget();
    }

    public void ExitInteraction()
    {
        stateMachine.GoToState(PlayerStateType.IDLE);
    }

    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis()
    {
        synthesisEnabled = !synthesisEnabled;
        //SynthesisManager.Instance.Show(synthesisEnabled);
        stateMachine.GoToState(synthesisEnabled ? PlayerStateType.INTERACTION : PlayerStateType.IDLE);
    }
    #endregion

}


