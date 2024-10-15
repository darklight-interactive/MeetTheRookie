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


/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>
[RequireComponent(typeof(MTRPlayerController))]
[RequireComponent(typeof(MTRPlayerInteractor))]
[RequireComponent(typeof(MTRPlayerAnimator))]
public class MTRPlayerInput : MonoBehaviour
{
    bool _sceneListenerInitialized = false;

    [Header("Enabled States")]
    [SerializeField, ShowOnly] bool _allInputEnabled;
    [SerializeField, ShowOnly] bool _movementInputEnabled;
    [SerializeField, ShowOnly] bool _interactInputEnabled;

    [Header("Active Inputs")]
    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;
    [SerializeField, ShowOnly] bool _activePrimaryInteractInput = false;

    public MTRPlayerController Controller => GetComponent<MTRPlayerController>();
    public MTRPlayerInteractor Interactor => GetComponent<MTRPlayerInteractor>();
    public bool IsAllInputEnabled
    {
        get
        {
            return _allInputEnabled = _movementInputEnabled && _interactInputEnabled;
        }
    }

    #region ---- < PRIVATE_METHODS > ( UNITY_RUNTIME ) --------------------------------- 
    void Awake()
    {
        SetAllInputsEnabled(false);
    }

    void OnDestroy()
    {
        SetAllInputsEnabled(false);
    }

    void Update()
    {
        _allInputEnabled = _movementInputEnabled && _interactInputEnabled;
    }


    #endregion

    [Button]
    public void SetAllInputsEnabled(bool enable)
    {
        if (enable)
        {
            _allInputEnabled = true;
            if (!_movementInputEnabled)
                SetMovementInputEnabled(true);
            if (!_interactInputEnabled)
                SetInteractInputEnabled(true);
        }
        else
        {
            _allInputEnabled = false;
            if (_movementInputEnabled)
                SetMovementInputEnabled(false);
            if (_interactInputEnabled)
                SetInteractInputEnabled(false);
        }
    }

    public void SetMovementInputEnabled(bool enable)
    {
        if (enable && !_movementInputEnabled)
        {
            UniversalInputManager.OnMoveInput += HandleMoveInput;
            UniversalInputManager.OnMoveInputCanceled += HandleOnMoveInputCanceled;
        }
        else if (!enable && _movementInputEnabled)
        {
            UniversalInputManager.OnMoveInput -= HandleMoveInput;
            UniversalInputManager.OnMoveInputCanceled -= HandleOnMoveInputCanceled;
        }
        _movementInputEnabled = enable;
        Debug.Log("MTRPlayerInput :: SetMovementInputEnabled :: " + enable, this);
    }

    public void SetInteractInputEnabled(bool enable)
    {
        if (enable && !_interactInputEnabled)
        {
            UniversalInputManager.OnPrimaryInteract += HandlePrimaryInteract;
            UniversalInputManager.OnPrimaryInteractCanceled += HandlePrimaryInteractCanceled;
        }
        else if (!enable && _interactInputEnabled)
        {
            UniversalInputManager.OnPrimaryInteract -= HandlePrimaryInteract;
            UniversalInputManager.OnPrimaryInteractCanceled -= HandlePrimaryInteractCanceled;
        }
        _interactInputEnabled = enable;
        Debug.Log("MTRPlayerInput :: SetInteractInputEnabled :: " + enable, this);
    }

    void HandleMoveInput(Vector2 moveInput)
    {
        Controller.HandleMoveInput(moveInput);
        _activeMoveInput = moveInput;
    }

    void HandleOnMoveInputCanceled()
    {
        Controller.HandleOnMoveInputCanceled();
        _activeMoveInput = Vector2.zero;
    }

    void HandlePrimaryInteract()
    {
        Interactor.InteractWithTarget();
        _activePrimaryInteractInput = true;
    }

    void HandlePrimaryInteractCanceled()
    {
        _activePrimaryInteractInput = false;
    }


    // =================================== [[ TRIGGER ]] =================================== >>
    /*
    void OnTriggerEnter2D(Collider2D other)
    {

        // Get Hidden Object Component
        Hideable_Object hiddenObject = other.GetComponent<Hideable_Object>();
        if (hiddenObject != null)
        {
            // debug.log for proof
            Debug.Log("Character is hidden");
            StateMachine.GoToState(MTRPlayerState.HIDE);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset state to Walk/Idle 
        if (other.GetComponent<Hideable_Object>() != null)
        {
            StateMachine.GoToState(MTRPlayerState.IDLE);
        }
    }





    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis()
    {
        synthesisEnabled = !synthesisEnabled;
        MTR_UIManager.Instance.synthesisManager.Show(synthesisEnabled);
        StateMachine.GoToState(synthesisEnabled ? MTRPlayerState.INTERACTION : MTRPlayerState.IDLE);
    }
    #endregion
*/


#if UNITY_EDITOR
    [CustomEditor(typeof(MTRPlayerInput))]
    public class PlayerControllerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        MTRPlayerInput _script;
        private ButtonsDrawer _buttonsDrawer;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRPlayerInput)target;
            _buttonsDrawer = new ButtonsDrawer(target);

            //_script.Awake();
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





