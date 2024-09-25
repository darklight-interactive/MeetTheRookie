using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using Codice.CM.SEIDInfo;
using Darklight.UnityExt.Utility;
using System.Linq;
using Darklight.UnityExt.Library;
using System;
using Unity.Android.Gradle.Manifest;



#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public partial class Interactable : MonoBehaviour,
    IInteractable, IUnityEditorListener
{
    public const string PREFIX = "<INTRCT>";

    readonly List<IInteractable.State> VALID_INTERACTION_STATES = new List<IInteractable.State>
        {
            IInteractable.State.TARGET,
            IInteractable.State.START,
            IInteractable.State.CONTINUE
        };

    StateMachine _stateMachine;

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
    protected InternalData data { get => _data; set => _data = value; }
    protected StateMachine stateMachine => _stateMachine ??= new StateMachine(this);
    public string Name { get => data.Name; }
    public string Key { get => data.Key; }
    public string Layer
    {
        get => data.Layer = gameObject.layer.ToString();
        set
        {
            data.Layer = value;
            gameObject.layer = LayerMask.NameToLayer(value);
        }
    }
    public IInteractable.State CurrentState { get => _currentState; }
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
        if (data == null)
            data = new InternalData(this);
        data.Preload(this);
    }

    public virtual void Initialize()
    {
        if (data.Initialize())
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
            _stateMachine = new StateMachine(this);

            // << SUBSCRIBE TO STATE CHANGES >> ------------------------------------
            _stateMachine.OnStateChanged += (IInteractable.State state) =>
            {
                _currentState = state;
            };

            // << GO TO READY STATE >> ------------------------------------
            stateMachine.GoToState(IInteractable.State.READY);
        }
    }

    public virtual void Refresh()
    {
        // << Refresh Serialized Fields >> ------------------------------------
        //_currentState = CurrentState;
    }

    public virtual void Reset()
    {
        if (stateMachine == null)
            return;
        stateMachine.GoToState(IInteractable.State.READY);
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
        stateMachine.GoToState(IInteractable.State.TARGET);
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
                stateMachine.GoToState(IInteractable.State.CONTINUE, true);
                break;
            case IInteractable.State.COMPLETE:
            case IInteractable.State.DISABLED:
                Debug.LogError($"{PREFIX} {Name} :: Cannot interact in state: {CurrentState}");
                break;
            default:
                stateMachine.GoToState(IInteractable.State.START);
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
            _script.Preload();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // << BUTTONS >> ------------------------------------
            if (!_script.data.IsPreloaded)
            {
                if (GUILayout.Button("Preload"))
                {
                    _script.Preload();
                }
            }
            else if (!_script.data.IsInitialized)
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


