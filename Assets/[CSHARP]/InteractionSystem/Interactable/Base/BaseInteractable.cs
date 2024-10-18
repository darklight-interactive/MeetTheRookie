using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using System;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

#region ==== < MAIN_CLASS > [[ BASE INTERACTABLE ]] ================================== >>>>
public partial class BaseInteractable : Interactable<BaseInteractable.InternalData, BaseInteractable.InternalStateMachine, BaseInteractable.State, BaseInteractable.Type>,
    IUnityEditorListener
{
    public enum Type
    {
        INTERACTABLE,
        INTERACTOR
    }

    public enum State
    {
        NULL,
        READY,
        TARGET,
        START,
        CONTINUE,
        COMPLETE,
        DISABLED
    }

    protected readonly List<State> VALID_INTERACTION_STATES = new List<State>
    {
        State.TARGET,
        State.START,
        State.CONTINUE
    };

    [SerializeField, ShowOnly] bool _isPreloaded;
    [SerializeField, ShowOnly] bool _isRegistered;
    [SerializeField, ShowOnly] bool _isInitialized;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] InteractionRequestDataObject _request;
    [SerializeField] InteractionRecieverLibrary _recievers;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] protected InternalData data;
    [SerializeField] protected InternalStateMachine stateMachine;

    public override string Name
    {
        get
        {
            if (data == null) return DEFAULT_NAME;
            return data.Name;
        }
    }
    public override string Key
    {
        get
        {
            if (data == null) return DEFAULT_KEY;
            return data.Key;
        }
    }
    public override string Layer
    {
        get
        {
            if (data == null) return DEFAULT_LAYER;
            return data.Layer;
        }
    }
    public override InteractionRequestDataObject Request
    {
        get
        {
            return _request;
        }
        protected set => _request = value;
    }
    public override InteractionRecieverLibrary Recievers
    {
        get
        {
            return _recievers;
        }
        protected set => _recievers = value;
    }
    public override bool IsPreloaded
    {
        get
        {
            return _isPreloaded;
        }
        protected set => _isPreloaded = value;
    }
    public override bool IsRegistered
    {
        get
        {
            return _isRegistered;
        }
        protected set => _isRegistered = value;
    }
    public override bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
        protected set => _isInitialized = value;
    }


    public override InternalData Data => data;
    public override InternalStateMachine StateMachine => stateMachine;
    public override State CurrentState
    {
        get
        {
            if (StateMachine == null) return State.NULL;
            return StateMachine.CurrentState;
        }
    }
    public override Type TypeKey => throw new NotImplementedException();

    #region ======== <PRIVATE_METHODS> ================================== >>>>

    protected virtual void PreloadData()
    {
        if (data == null)
            data = new InternalData(this, DEFAULT_NAME, DEFAULT_KEY);
        else
            data.LoadData(this);
    }

    protected virtual void PreloadStateMachine()
    {
        // << CREATE THE STATE MACHINE >> ------------------------------------
        stateMachine = new InternalStateMachine(this);
    }

    protected virtual bool ValidateRegister(bool logErrors = false)
    {
        bool isValid = IsRegistered
            && Recievers.Count > 0
            && !Recievers.HasUnsetKeysOrValues();

        if (!isValid && logErrors)
        {
            string outLog = $"{PREFIX} {Name} :: Preload Validation Failed";
            if (!IsRegistered)
                outLog += " :: Not Registered";
            if (Recievers.Count == 0)
                outLog += " :: No Recievers Requested";
            if (Recievers.HasUnsetKeysOrValues())
                outLog += " :: Found Unset Recievers";
            Debug.LogError(outLog, this);
        }

        return isValid;
    }
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
    public override void Preload()
    {
        PreloadData();
    }

    public override void Register()
    {
        // << VALIDATE REGISTRATION >> ------------------------------------
        if (!ValidateRegister(true))
            return;

        // << REGISTER INTERACTABLE >> ------------------------------------
        InteractionSystem.Registry.TryRegisterInteractable(this, out bool result);
        IsRegistered = result;
    }

    public override void Initialize()
    {
        // << SET TO READY STATE >> ------------------------------------
        stateMachine.GoToState(State.READY);
    }

    public override void Refresh()
    {
        StateMachine.Step();
    }

    public override void Reset()
    {
        if (StateMachine == null)
            return;
        StateMachine.GoToState(State.READY);
    }

    public override bool AcceptTarget(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        // If not forced, check to make sure the interactable is in a valid state
        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (CurrentState != State.READY) return false;
        }

        // << ACCEPT TARGET >> ------------------------------------
        StateMachine.GoToState(State.TARGET);
        return true;
    }

    public override bool AcceptInteraction(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (!VALID_INTERACTION_STATES.Contains(CurrentState)) return false;
        }

        // << ACCEPT INTERACTION >> ------------------------------------
        Debug.Log($"{PREFIX} {Name} :: AcceptInteraction from {interactor}", this);
        switch (CurrentState)
        {
            case State.START:
            case State.CONTINUE:
                StateMachine.GoToState(State.CONTINUE, true);
                break;
            case State.COMPLETE:
            case State.DISABLED:
                break;
            default:
                StateMachine.GoToState(State.START);
                break;
        }

        return true;
    }
    #endregion

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseInteractable), true)]
    public class InteractableCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        BaseInteractable _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (BaseInteractable)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // << DRAW DEFAULT INSPECTOR >> ------------------------------------
            base.OnInspectorGUI();

            // << BUTTONS >> ------------------------------------
            if (!_script.IsPreloaded)
            {
                if (GUILayout.Button("Preload"))
                {
                    _script.Preload();
                }
            }
            else if (!_script.IsInitialized)
            {
                if (GUILayout.Button("Initialize"))
                {
                    _script.Initialize();
                }
            }



            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}

#endregion
