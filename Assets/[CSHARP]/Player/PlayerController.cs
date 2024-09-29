using System;
using System.Collections.Generic;
using Darklight.UnityExt.Input;

using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using static Darklight.UnityExt.Animation.FrameAnimationPlayer;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PlayerState { NULL, IDLE, WALK, INTERACTION, HIDE, WALKOVERRIDE }
public enum PlayerFacing { RIGHT, LEFT }

/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>
[RequireComponent(typeof(PlayerController), typeof(MTRPlayerInteractor), typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour
{
    SceneBounds _sceneBounds;
    CurrentDestinationPoint _destinationPoint = new CurrentDestinationPoint();

    [Header("Debug")]
    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;
    [SerializeField] PlayerFacing _facing;

    [Header("Settings")]
    [Range(0.1f, 5f)] public float playerSpeed = 1f;

    public MTRPlayerInteractor Interactor { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerState CurrentState => StateMachine.CurrentState;
    public CurrentDestinationPoint DestinationPoint => _destinationPoint;
    public PlayerFacing Facing => _facing;

    public void Awake()
    {
        Interactor = GetComponent<MTRPlayerInteractor>();
        Animator = GetComponent<PlayerAnimator>();
        Animator.SetFacing(SpriteDirection.RIGHT);

        WalkOverride walkOverride = new WalkOverride(StateMachine, PlayerState.WALKOVERRIDE, _destinationPoint);

        StateMachine = new PlayerStateMachine(new Dictionary<PlayerState, FiniteState<PlayerState>> {
            {PlayerState.NULL, new FinitePlayerState(StateMachine, PlayerState.NULL)},
            {PlayerState.IDLE, new FinitePlayerState(StateMachine, PlayerState.IDLE)},
            {PlayerState.WALK, new FinitePlayerState(StateMachine, PlayerState.WALK)},
            {PlayerState.INTERACTION, new FinitePlayerState(StateMachine, PlayerState.INTERACTION)},
            {PlayerState.HIDE, new FinitePlayerState(StateMachine, PlayerState.HIDE)},
            {PlayerState.WALKOVERRIDE, walkOverride}
        }, PlayerState.IDLE, this);

        walkOverride._stateMachine = StateMachine;
    }

    void Start()
    {
        UniversalInputManager.OnMoveInput += (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputCanceled += () => _activeMoveInput = Vector2.zero;
        UniversalInputManager.OnPrimaryInteract += () => Interactor.InteractWithTarget();
        UniversalInputManager.OnSecondaryInteract += ToggleSynthesis;

        // << Find SceneBounds >>
        SceneBounds[] bounds = FindObjectsByType<SceneBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (bounds.Length > 0)
        {
            _sceneBounds = bounds[0];
        }
        else
        {
            _sceneBounds = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Step();
        HandleMovement();
    }

    public void OnDestroy()
    {
        UniversalInputManager.OnMoveInput -= (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputCanceled -= () => _activeMoveInput = Vector2.zero;
        UniversalInputManager.OnPrimaryInteract -= () => Interactor.InteractWithTarget();
        UniversalInputManager.OnSecondaryInteract -= ToggleSynthesis;
    }

    void HandleMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (CurrentState == PlayerState.INTERACTION) return;
        if (CurrentState == PlayerState.WALKOVERRIDE) return;

        // << HANDLE INPUT >>
        Vector2 moveDirection = new Vector2(_activeMoveInput.x, 0); // Get the horizontal input
        moveDirection *= playerSpeed; // Scalar

        // << SET FACING >>
        if (moveDirection.x > 0) _facing = PlayerFacing.RIGHT;
        if (moveDirection.x < 0) _facing = PlayerFacing.LEFT;

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
        if (Facing == PlayerFacing.RIGHT)
            Animator.SetFacing(SpriteDirection.RIGHT);
        else if (Facing == PlayerFacing.LEFT)
            Animator.SetFacing(SpriteDirection.LEFT);

        // Update the State Machine
        if (moveDirection.magnitude > 0.1f)
            StateMachine.GoToState(PlayerState.WALK);
        else
            StateMachine.GoToState(PlayerState.IDLE);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (StateMachine.CurrentState == PlayerState.INTERACTION) return;

        // Get Hidden Object Component
        Hideable_Object hiddenObject = other.GetComponent<Hideable_Object>();
        if (hiddenObject != null)
        {
            // debug.log for proof
            Debug.Log("Character is hidden");
            StateMachine.GoToState(PlayerState.HIDE);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset state to Walk/Idle 
        if (other.GetComponent<Hideable_Object>() != null)
        {
            StateMachine.GoToState(PlayerState.IDLE);
        }
    }

    /// <summary>
    /// Moves the player into the interaction state
    /// </summary>
    public void EnterInteraction()
    {
        if (StateMachine == null) return;
        StateMachine.GoToState(PlayerState.INTERACTION);
        //Debug.Log("Player Controller :: Enter Interaction");
    }

    /// <summary>
    /// Removes the player from the interaction state
    /// </summary>
    public void ExitInteraction()
    {
        StateMachine.GoToState(PlayerState.IDLE);
        //Debug.Log("Player Controller :: Exit Interaction");
    }


    #region  [[ STATE MACHINE ]] ======================================================== >>

    public class FinitePlayerState : FiniteState<PlayerState>
    {
        /// <param name="args">
        ///   args[0] = PlayerController ( playerController )
        public FinitePlayerState(PlayerStateMachine stateMachine, PlayerState stateType) : base(stateMachine, stateType) { }

        public override void Enter()
        {
            // Debug.Log($"Entering State: {stateType}");
        }

        public override void Exit()
        {
            // Debug.Log($"Exiting State: {stateType}");
        }

        public override void Execute()
        {
            // Debug.Log($"Executing State: {stateType}");
        }
    }

    public class WalkOverride : FinitePlayerState
    {
        public PlayerStateMachine _stateMachine;
        private float _walkDestinationX;
        private CurrentDestinationPoint _currentDestinationPoint;

        public WalkOverride(PlayerStateMachine stateMachine, PlayerState stateType, CurrentDestinationPoint destination) : base(stateMachine, stateType)
        {
            _stateMachine = stateMachine;
            _currentDestinationPoint = destination;
        }

        public override void Enter()
        {
            // Debug.Log($"Entering State: WALK");
        }

        public override void Exit()
        {
            // Debug.Log($"Exiting State: {stateType}");
        }

        public override void Execute()
        {
            float _walkDirection = 0;
            float _walkSpeed = 1;
            Transform transform = _stateMachine._controller.transform;
            _walkDestinationX = _currentDestinationPoint.destinationPoint.x;

            if (Mathf.Abs(transform.position.x - _walkDestinationX) < .1)
            {
                _stateMachine.GoToState(PlayerState.IDLE);
                return;
            }

            if (transform.position.x > _walkDestinationX)
            {
                _walkDirection = -1;
            }
            else
            {
                _walkDirection = 1;
            }

            float movement = _walkDirection * _walkSpeed;
            float targetX = transform.position.x + movement;

            // move the character
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

            // Update the Animation
            //_stateMachine._animator.FrameAnimationPlayer.FlipSprite(new Vector2(_walkDirection, 0));
        }

    }

    #endregion

    #region [[ DESTINATION POINT ]] ======================================================== >>

    public class CurrentDestinationPoint
    {
        public DestinationPoint destinationPoint;

        public CurrentDestinationPoint()
        {
            destinationPoint = null;
        }
    }
    #endregion


    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis()
    {
        synthesisEnabled = !synthesisEnabled;
        MTR_UIManager.Instance.synthesisManager.Show(synthesisEnabled);
        StateMachine.GoToState(synthesisEnabled ? PlayerState.INTERACTION : PlayerState.IDLE);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    PlayerController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (PlayerController)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif




