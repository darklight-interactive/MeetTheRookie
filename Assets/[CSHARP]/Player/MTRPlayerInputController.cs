using System;
using System.Collections.Generic;
using Darklight.UnityExt.Input;

using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using static Darklight.UnityExt.Animation.FrameAnimationPlayer;
using EasyButtons;


#if UNITY_EDITOR
using UnityEditor;
using EasyButtons.Editor;
#endif

public enum PlayerState { NULL, IDLE, WALK, INTERACTION, HIDE, WALKOVERRIDE }
public enum PlayerFacing { RIGHT, LEFT }

/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>
[RequireComponent(typeof(MTRPlayerInputController), typeof(MTRPlayerInteractor), typeof(PlayerAnimator))]
public class MTRPlayerInputController : MonoBehaviour
{
    SceneBounds _sceneBounds;
    PlayerStateMachine _stateMachine;
    float _speed = 1f;

    [Header("Settings")]
    [SerializeField] PlayerFacing _facing;

    [Header("Debug")]
    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;
    [SerializeField, ShowOnly] bool _inputEnabled;

    // =================================== [[ PROPERTIES ]] =================================== >>
    public MTRPlayerInteractor Interactor { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerStateMachine StateMachine
    {
        get
        {
            if (_stateMachine == null)
                _stateMachine = new PlayerStateMachine(this);
            return _stateMachine;
        }
    }
    public PlayerState CurrentState
    {
        get
        {
            try
            {
                return StateMachine.CurrentState;
            }
            catch (Exception e)
            {
                return PlayerState.NULL;
            }
        }
    }
    public PlayerFacing Facing => _facing;

    public void Awake()
    {
        MTRSceneManager.Controller.StateMachine.OnStateChanged += OnSceneStateChanged;


        Interactor = GetComponent<MTRPlayerInteractor>();
        Animator = GetComponent<PlayerAnimator>();
        Animator.SetFacing(SpriteDirection.RIGHT);
    }

    void OnSceneStateChanged(MTRSceneState state)
    {
        switch (state)
        {
            case MTRSceneState.PLAY_MODE:
                SetInputEnabled(true);
                break;
            default:
                SetInputEnabled(false);
                break;
        }
    }

    void Start()
    {
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
        UpdateMovement();
    }

    public void OnDestroy()
    {
        SetInputEnabled(false);
    }

    [Button]
    public void SetInputEnabled(bool enable)
    {
        if (enable)
        {
            _inputEnabled = true;
            UniversalInputManager.OnMoveInput += HandleOnMoveInput;
            UniversalInputManager.OnMoveInputCanceled += HandleOnMoveInputCanceled;
            UniversalInputManager.OnPrimaryInteract += HandlePrimaryInteract;
            UniversalInputManager.OnSecondaryInteract += ToggleSynthesis;
        }
        else
        {
            _inputEnabled = false;
            UniversalInputManager.OnMoveInput -= HandleOnMoveInput;
            UniversalInputManager.OnMoveInputCanceled -= HandleOnMoveInputCanceled;
            UniversalInputManager.OnPrimaryInteract -= HandlePrimaryInteract;
            UniversalInputManager.OnSecondaryInteract -= ToggleSynthesis;
        }
    }

    void HandleOnMoveInput(Vector2 input)
    {
        _activeMoveInput = input;
    }

    void HandleOnMoveInputCanceled()
    {
        _activeMoveInput = Vector2.zero;
    }

    void HandlePrimaryInteract()
    {
        Interactor.InteractWithTarget();
    }

    // =================================== [[ MOVEMENT ]] =================================== >>
    void UpdateMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (CurrentState == PlayerState.INTERACTION) return;
        if (CurrentState == PlayerState.WALKOVERRIDE) return;

        // << HANDLE INPUT >>
        Vector2 moveDirection = new Vector2(_activeMoveInput.x, 0); // Get the horizontal input
        moveDirection *= _speed; // Scalar

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


    // =================================== [[ TRIGGER ]] =================================== >>

    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (StateMachine == null) return;
        if (StateMachine.CurrentState == PlayerState.INTERACTION) return;
        */

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




    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis()
    {
        synthesisEnabled = !synthesisEnabled;
        MTR_UIManager.Instance.synthesisManager.Show(synthesisEnabled);
        StateMachine.GoToState(synthesisEnabled ? PlayerState.INTERACTION : PlayerState.IDLE);
    }
    #endregion


#if UNITY_EDITOR
    [CustomEditor(typeof(MTRPlayerInputController))]
    public class PlayerControllerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        MTRPlayerInputController _script;
        private ButtonsDrawer _buttonsDrawer;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRPlayerInputController)target;
            _buttonsDrawer = new ButtonsDrawer(target);

            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();
            _buttonsDrawer.DrawButtons(targets);

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}





