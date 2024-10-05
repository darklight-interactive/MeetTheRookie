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
[RequireComponent(typeof(PlayerAnimator))]
public class MTRPlayerInput : MonoBehaviour
{
    bool _sceneListenerInitialized = false;
    [SerializeField, ShowOnly] bool _allInputEnabled;
    [SerializeField, ShowOnly] bool _movementInputEnabled;
    [SerializeField, ShowOnly] bool _interactInputEnabled;

    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;

    public MTRPlayerController Controller => GetComponent<MTRPlayerController>();
    public MTRPlayerInteractor Interactor => GetComponent<MTRPlayerInteractor>();
    public bool IsAllInputEnabled => _allInputEnabled;


    // =================================== [[ PROPERTIES ]] =================================== >>
    void Awake()
    {
        SetAllInputsEnabled(false);
    }


    void Start()
    {
        if (Application.isPlaying)
        {
            MTRSceneController.StateMachine.OnStateChanged += OnSceneStateChanged;
        }
    }

    void Update()
    {

    }

    void OnSceneStateChanged(MTRSceneState state)
    {
        switch (state)
        {
            case MTRSceneState.PLAY_MODE:
                SetAllInputsEnabled(true);
                Interactor.SetEnabled(true);
                break;
            default:
                SetAllInputsEnabled(false);
                break;
        }
    }


    // Update is called once per frame


    public void OnDestroy()
    {
        MTRSceneController.StateMachine.OnStateChanged -= OnSceneStateChanged;
        SetAllInputsEnabled(false);
    }

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
            UniversalInputManager.OnMoveInput += Controller.HandleMoveInput;
            UniversalInputManager.OnMoveInputCanceled += Controller.HandleOnMoveInputCanceled;
        }
        else if (!enable && _movementInputEnabled)
        {
            UniversalInputManager.OnMoveInput -= Controller.HandleMoveInput;
            UniversalInputManager.OnMoveInputCanceled -= Controller.HandleOnMoveInputCanceled;
        }
        _movementInputEnabled = enable;
    }

    public void SetInteractInputEnabled(bool enable)
    {
        if (enable && !_interactInputEnabled)
            UniversalInputManager.OnPrimaryInteract += HandlePrimaryInteract;
        else if (!enable && _interactInputEnabled)
            UniversalInputManager.OnPrimaryInteract -= HandlePrimaryInteract;
        _interactInputEnabled = enable;
    }

    void HandlePrimaryInteract()
    {
        Interactor.InteractWithTarget();
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





