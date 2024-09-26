using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using Darklight.UnityExt.Library;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public partial class Interactable : MonoBehaviour,
    IInteractable, IUnityEditorListener
{
    public const string PREFIX = "INTRCT";

    readonly List<IInteractable.State> VALID_INTERACTION_STATES = new List<IInteractable.State>
        {
            IInteractable.State.TARGET,
            IInteractable.State.START,
            IInteractable.State.CONTINUE
        };

    InternalStateMachine _stateMachine;

    [Header("Internal Data")]
    [SerializeField] InternalData _data;

    [Header("State Machine Data")]
    [SerializeField, ShowOnly] IInteractable.State _currentState;

    [Header("Interaction Recievers")]
    [SerializeField]
    EnumComponentLibrary<InteractionTypeKey, InteractionReciever> _recievers = new EnumComponentLibrary<InteractionTypeKey, InteractionReciever>()
    {
        ReadOnlyKey = true,
        ReadOnlyValue = true,
        RequiredKeys = new InteractionTypeKey[]
        {
            InteractionTypeKey.TARGET
        },
    };

    #region ======== [[ PROPERTIES ]] ================================== >>>>
    public InternalData Data { get => _data; set => _data = value; }
    public InternalStateMachine StateMachine => _stateMachine ??= new InternalStateMachine(this);
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
    public IInteractable.State CurrentState { get => _currentState; }
    public EnumObjectLibrary<InteractionTypeKey, InteractionReciever> Recievers { get => _recievers; }
    #endregion

    #region ======== [[ EVENTS ]] ================================== >>>>
    public event IInteractable.InteractionEvent OnReadyEvent;
    public event IInteractable.InteractionEvent OnTargetEvent;
    public event IInteractable.InteractionEvent OnStartEvent;
    public event IInteractable.InteractionEvent OnContinueEvent;
    public event IInteractable.InteractionEvent OnCompleteEvent;
    public event IInteractable.InteractionEvent OnDisabledEvent;
    #endregion

    #region ======== <PRIVATE_METHODS> [[ UNITY RUNTIME ]] ================================== >>>>
    void Awake() => Preload();
    void Start() => Initialize();
    void Update() => Refresh();
    void OnDrawGizmos()
    {
        Vector2 labelPos = (Vector2)transform.position + (Vector2.up * 0.25f);
        CustomGizmos.DrawLabel(CurrentState.ToString(), labelPos, new GUIStyle()
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState() { textColor = Color.white }
        });
    }
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
    public virtual void Preload()
    {
        if (Data == null)
            Data = new InternalData(this);
        Data.Preload(this);
    }

    public virtual void Initialize()
    {
        if (Data.Initialize())
        {
            // << SUBSCRIBE TO EVENTS >> ------------------------------------
            /*
            OnReadyEvent += () => Debug.Log($"{PREFIX} {Name} :: OnReadyEvent");
            OnTargetEvent += () => Debug.Log($"{PREFIX} {Name} :: OnTargetEvent");
            OnStartEvent += () => Debug.Log($"{PREFIX} {Name} :: OnStartEvent");
            OnContinueEvent += () => Debug.Log($"{PREFIX} {Name} :: OnContinueEvent");
            OnCompleteEvent += () => Debug.Log($"{PREFIX} {Name} :: OnCompleteEvent");
            OnDisabledEvent += () => Debug.Log($"{PREFIX} {Name} :: OnDisabledEvent");
            */

            // << CREATE THE STATE MACHINE >> ------------------------------------
            _stateMachine = new InternalStateMachine(this);

            // << SUBSCRIBE TO STATE CHANGES >> ------------------------------------
            _stateMachine.OnStateChanged += (IInteractable.State state) =>
            {
                _currentState = _stateMachine.CurrentState;
            };

            // << GO TO READY STATE >> ------------------------------------
            StateMachine.GoToState(IInteractable.State.READY);
        }
        else
        {
            Invoke(nameof(Initialize), 0.25f);
            Debug.LogError($"{PREFIX} {Name} :: Failed to initialize, trying again", this);
        }
    }

    public virtual void Refresh()
    {
        // << Refresh Serialized Fields >> ------------------------------------
        //_currentState = CurrentState;
    }

    public virtual void Reset()
    {
        if (StateMachine == null)
            return;
        StateMachine.GoToState(IInteractable.State.READY);
    }

    public virtual bool AcceptTarget(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        // If not forced, check to make sure the interactable is in a valid state
        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (CurrentState != IInteractable.State.READY) return false;
        }

        // << ACCEPT TARGET >> ------------------------------------
        StateMachine.GoToState(IInteractable.State.TARGET);
        return true;
    }

    public virtual bool AcceptInteraction(IInteractor interactor, bool force = false)
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
            case IInteractable.State.START:
            case IInteractable.State.CONTINUE:
                StateMachine.GoToState(IInteractable.State.CONTINUE, true);
                break;
            case IInteractable.State.COMPLETE:
            case IInteractable.State.DISABLED:
                Debug.LogError($"{PREFIX} {Name} :: Cannot interact in state: {CurrentState}");
                break;
            default:
                StateMachine.GoToState(IInteractable.State.START);
                break;
        }

        return true;
    }
    #endregion

    /*
    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }
    */

#if UNITY_EDITOR
    [CustomEditor(typeof(Interactable), true)]
    public class InteractableCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        Interactable _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (Interactable)target;
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


