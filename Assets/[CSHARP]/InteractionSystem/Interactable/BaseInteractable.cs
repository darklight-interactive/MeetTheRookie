using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using Darklight.UnityExt.Library;
using System;
using UnityEditorInternal;


#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteractable<TTypeEnum, TStateEnum>
    where TTypeEnum : Enum
    where TStateEnum : Enum
{
    string Name { get; }
    string Key { get; }
    string Layer { get; set; }
    TTypeEnum Type { get; }
    TStateEnum CurrentState { get; }
    InteractionRecieverLibrary Recievers { get; }

    // ===================== [[ METHODS ]] =====================
    void Preload();
    void Initialize();
    void Refresh();
    void Reset();

    bool AcceptTarget(IInteractor interactor, bool force = false);
    bool AcceptInteraction(IInteractor interactor, bool force = false);
}

public enum BaseInteractableState
{
    NULL,
    READY,
    TARGET,
    START,
    CONTINUE,
    COMPLETE,
    DISABLED
}

public enum BaseInteractableType
{
    BASE,
    INTERACTOR,
    NPC,
    PLAYER
}


/// <summary>
/// This is the base interactable interface that implements the Base Type and State Enums
/// </summary>
public interface IBaseInteractable : IInteractable<BaseInteractableType, BaseInteractableState> { }


/// <summary>
/// This is the base interactable class uses the BaseInteractableData and BaseInteractableStateMachine
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TStateMachine"></typeparam>
[System.Serializable]
public abstract class BaseInteractable<TData, TStateMachine> : MonoBehaviour, IBaseInteractable
    where TData : BaseInteractableData
    where TStateMachine : BaseInteractableStateMachine
{
    protected const string PREFIX = "INTRCBL";
    protected readonly List<BaseInteractableState> VALID_INTERACTION_STATES = new List<BaseInteractableState>
    {
        BaseInteractableState.TARGET,
        BaseInteractableState.START,
        BaseInteractableState.CONTINUE
    };

    [SerializeField] protected TData data;
    [SerializeField] protected TStateMachine stateMachine;
    [SerializeField] protected InteractionRecieverLibrary recievers;

    #region ======== [[ PROPERTIES ]] ================================== >>>>
    public TData Data => data;
    public TStateMachine StateMachine => stateMachine;
    public InteractionRecieverLibrary Recievers => recievers;
    public string Name { get => Data.Name; }
    public string Key { get => Data.Key; }
    public string Layer
    {
        get => Data.Layer = gameObject.layer.ToString();
        set
        {
            Data.Layer = value;
            gameObject.layer = LayerMask.NameToLayer(value);
        }
    }
    public BaseInteractableState CurrentState
    {
        get
        {
            if (StateMachine == null)
                return BaseInteractableState.NULL;
            return StateMachine.CurrentState;
        }
    }

    public BaseInteractableType Type => throw new NotImplementedException();

    BaseInteractableState IInteractable<BaseInteractableType, BaseInteractableState>.CurrentState => throw new NotImplementedException();
    #endregion

    #region [[ EVENTS ]] <PUBLIC> ================================== >>>>
    public delegate void InteractionEvent();
    #endregion

    #region [[ UNITY_METHODS ]]
    protected void Awake() => Preload();
    protected void Start() => Initialize();
    protected void Update() => Refresh();
    #endregion

    /// <summary>
    /// Preload the interactable with core data & subscriptions <br/>
    /// This is called when the interactable is first created, typically in Awake()
    /// </summary>
    public abstract void Preload();
    /// <summary>
    /// Initialize the interactable within the scene by 
    /// storing scene specific references and data.
    /// This is called when the interactable is first created, typically in Start()
    /// </summary>
    public abstract void Initialize();
    /// <summary>
    /// Refresh the interactable to update its state. <br/>
    /// This is called every frame, typically in Update()
    /// </summary>
    public abstract void Refresh();
    /// <summary>
    /// Reset the interactable to its default state
    /// </summary>
    public abstract void Reset();
    public abstract bool AcceptTarget(IInteractor interactor, bool force = false);
    public abstract bool AcceptInteraction(IInteractor interactor, bool force = false);

}

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class BaseInteractable : BaseInteractable<BaseInteractableData, BaseInteractableStateMachine>,
    IUnityEditorListener
{
    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
    public override void Preload()
    {
        if (data == null)
            data = new BaseInteractableData(this);
        data.Preload(this);
    }

    public override void Initialize()
    {
        if (data.Initialize())
        {
            // << CREATE THE STATE MACHINE >> ------------------------------------
            stateMachine = new BaseInteractableStateMachine(this);

            // << GO TO READY STATE >> ------------------------------------
            StateMachine.GoToState(BaseInteractableState.READY);
        }
        else
        {
            Invoke(nameof(Initialize), 0.25f);
            Debug.LogError($"{PREFIX} {Name} :: Failed to initialize, trying again", this);
        }
    }

    public override void Refresh()
    {
        StateMachine.Step();
    }

    public override void Reset()
    {
        if (StateMachine == null)
            return;
        StateMachine.GoToState(BaseInteractableState.READY);
    }

    public override bool AcceptTarget(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        // If not forced, check to make sure the interactable is in a valid state
        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (CurrentState != BaseInteractableState.READY) return false;
        }

        // << ACCEPT TARGET >> ------------------------------------
        StateMachine.GoToState(BaseInteractableState.TARGET);
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
            case BaseInteractableState.START:
            case BaseInteractableState.CONTINUE:
                StateMachine.GoToState(BaseInteractableState.CONTINUE, true);
                break;
            case BaseInteractableState.COMPLETE:
            case BaseInteractableState.DISABLED:
                Debug.LogError($"{PREFIX} {Name} :: Cannot interact in state: {CurrentState}");
                break;
            default:
                StateMachine.GoToState(BaseInteractableState.START);
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

            // << BUTTONS >> ------------------------------------
            if (!_script.Data.IsPreloaded)
            {
                if (GUILayout.Button("Preload"))
                {
                    _script.Preload();
                }
            }
            else if (!_script.Data.IsInitialized)
            {
                if (GUILayout.Button("Initialize"))
                {
                    _script.Initialize();
                }
            }

            // << DRAW DEFAULT INSPECTOR >> ------------------------------------
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}


