using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using Codice.CM.SEIDInfo;
using Darklight.UnityExt.Utility;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public partial class Interactable : MonoBehaviour,
    IInteractable, IUnityEditorListener
{
    const string PREFIX = "<INT>";
    const string DEFAULT_NAME = "DefaultName";
    const string DEFAULT_KEY = "DefaultKey";
    const string DEFAULT_LAYER = "Interactable";

    #region ---- <READONLY> [[ VALID_STATES ]] ------------------------------------ >>>>
    readonly List<IInteractable.State> VALID_TARGET_STATES = new List<IInteractable.State>
    {
        IInteractable.State.READY
    };

    readonly List<IInteractable.State> VALID_INTERACTION_STATES = new List<IInteractable.State>
    {
        IInteractable.State.TARGET,
        IInteractable.State.START,
        IInteractable.State.CONTINUE
    };

    #endregion

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider;
    StateMachine _stateMachine;

    [Header(" (( FLAGS )) -------- >>")]
    [SerializeField, ShowOnly] bool _isRegistered = false;
    [SerializeField, ShowOnly] bool _isPreloaded = false;
    [SerializeField, ShowOnly] bool _isInitialized = false;

    [Header(" (( VALUES )) -------- >>")]
    [SerializeField] string _name = DEFAULT_NAME;
    [SerializeField] string _key = DEFAULT_KEY;
    [SerializeField, ShowOnly] string _layer = DEFAULT_LAYER;
    [SerializeField, ShowOnly] IInteractable.State _currentState;
    [SerializeField, ShowAssetPreview] Sprite _sprite;

    [Header(" (( INTERACTION HANDLERS )) -------- >>")]
    [SerializeField] InteractionHandlerLibrary _handlerLibrary;

    [Header(" (( INTERACTION SETTINGS )) -------- >>")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;


    #region ======== [[ PROPERTIES ]] ================================== >>>>
    public string Name { get => _name; set => _name = value; }
    public string Key { get => _key; set => _key = value; }
    public string Layer
    {
        get => _layer = gameObject.layer.ToString();
        set
        {
            _layer = value;
            gameObject.layer = LayerMask.NameToLayer(value);
        }
    }
    public IInteractable.State CurrentState
    {
        get
        {
            if (_stateMachine == null)
                _currentState = IInteractable.State.NULL;
            else
            {
                try { _currentState = _stateMachine.CurrentState; }
                catch { _currentState = IInteractable.State.NULL; }
            }
            return _currentState;
        }
    }
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
    void Awake() => Initialize();
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

    #region ======== <PRIVATE_METHODS> [[ Internal Methods ]] ================================== >>>>
    void PreloadSpriteRenderer()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        // << SET THE INITIAL SPRITE >> ------------------------------------
        if (_sprite != null)
            _spriteRenderer.sprite = _sprite;
        else if (_spriteRenderer.sprite != null)
            _sprite = _spriteRenderer.sprite;

        // << SET THE DEFAULT TINT >> ------------------------------------
        _spriteRenderer.color = _defaultTint;
    }
    void PreloadBoxCollider()
    {
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
            _collider = this.gameObject.AddComponent<BoxCollider2D>();

        // << SET THE COLLIDER SIZE >> ------------------------------------
        // Set the collider size to half the size of the transform scale
        _collider.size = Vector2.one * transform.localScale.x * 0.5f;
    }

    void PreloadInteractionHandlers()
    {
        RemoveUnusedHandlers();
        List<InteractionTypeKey> keys = _handlerLibrary.Keys.ToList();
        foreach (InteractionTypeKey key in keys)
        {
            InteractionHandler currentHandlerValue = _handlerLibrary[key];
            if (currentHandlerValue == null)
            {
                InteractionHandler handlerInChild = GetHandlerInChildren(key);
                if (handlerInChild != null)
                {
                    _handlerLibrary[key] = handlerInChild;
                    continue;
                }

                GameObject handlerGO = InteractionSystem.Factory.CreateInteractionHandler(key);
                if (handlerGO == null) continue;

                InteractionHandler handler = handlerGO.GetComponent<InteractionHandler>();
                if (handler == null) continue;

                handler.transform.SetParent(this.transform);
                handler.transform.localPosition = Vector3.zero;
                handler.transform.localRotation = Quaternion.identity;
                handler.transform.localScale = Vector3.one;

                _handlerLibrary[key] = handler;
            }
        }

        Debug.Log($"Preloaded Interaction Handlers for {Name}. Count {_handlerLibrary.Count}", this);
    }

    InteractionHandler GetHandlerInChildren(InteractionTypeKey key)
    {
        InteractionHandler[] handlers = GetComponentsInChildren<InteractionHandler>();
        foreach (InteractionHandler handler in handlers)
        {
            if (handler.TypeKey == key)
                return handler;
        }
        return null;
    }

    void RemoveUnusedHandlers()
    {
        InteractionHandler[] interactionHandlers = GetComponentsInChildren<InteractionHandler>();
        foreach (InteractionHandler interactionHandler in interactionHandlers)
        {
            if (!_handlerLibrary.ContainsKey(interactionHandler.TypeKey) || _handlerLibrary[interactionHandler.TypeKey] != null)
            {
                ObjectUtility.DestroyAlways(interactionHandler.gameObject);
            }
        }
    }



    void UpdateGameObjectName()
    {
        // Set name to sprite name if Default, Default if null
        if (_name == string.Empty) _name = DEFAULT_NAME;
        if (_name == DEFAULT_NAME && _sprite != null) _name = _sprite.name;


        if (_key == string.Empty) _key = DEFAULT_KEY;
        if (_layer == string.Empty) _layer = DEFAULT_LAYER;
        this.gameObject.name = $"{PREFIX} {Key} : {Name}";
    }

    #endregion

    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>

    public virtual void Preload()
    {
        _isPreloaded = false;
        _isRegistered = false;
        _isInitialized = false;

        PreloadSpriteRenderer();
        PreloadBoxCollider();
        PreloadInteractionHandlers();

        // << REGISTER THE INTERACTABLE >> ------------------------------------
        _isRegistered = InteractionSystem.Registry.TryRegister(this);
        if (!_isRegistered)
        {
            Debug.LogError($"{PREFIX} {Name} :: Not registered with the Interaction System.");
            return;
        }

        UpdateGameObjectName();
    }

    public virtual void Initialize()
    {
        _isPreloaded = _isRegistered
            && !_handlerLibrary.HasUnsetKeysOrValues()
            && _handlerLibrary.Count > 0;
        if (!_isPreloaded)
        {
            if (!_isRegistered)
                Debug.LogError($"{PREFIX} {Name} :: Not registered with the Interaction System.");
            else if (_handlerLibrary.HasUnsetKeysOrValues())
                Debug.LogError($"{PREFIX} {Name} :: Interaction Handlers are not preloaded. Check for null or unset handlers.");
            else if (_handlerLibrary.Count == 0)
                Debug.LogError($"{PREFIX} {Name} :: Interaction Handlers are empty.");

            Preload();
            return;
        }

        // << SUBSCRIBE TO EVENTS >> ------------------------------------
        OnReadyEvent += () => Debug.Log($"{PREFIX} {Name} :: OnReadyEvent");
        OnTargetEvent += () => Debug.Log($"{PREFIX} {Name} :: OnTargetEvent");
        OnStartEvent += () => Debug.Log($"{PREFIX} {Name} :: OnStartEvent");
        OnContinueEvent += () => Debug.Log($"{PREFIX} {Name} :: OnContinueEvent");
        OnCompleteEvent += () => Debug.Log($"{PREFIX} {Name} :: OnCompleteEvent");
        OnDisabledEvent += () => Debug.Log($"{PREFIX} {Name} :: OnDisabledEvent");

        // << CREATE THE STATE MACHINE >> ------------------------------------
        _stateMachine = new StateMachine(this);

        // << SUBSCRIBE TO STATE CHANGES >> ------------------------------------
        _stateMachine.OnStateChanged += (IInteractable.State state) =>
        {
            _currentState = state;
        };

        // << GO TO READY STATE >> ------------------------------------
        _stateMachine.GoToState(IInteractable.State.READY);

        _isInitialized = true;
    }

    public virtual void Refresh()
    {
        // << Refresh Serialized Fields >> ------------------------------------
        //_currentState = CurrentState;
    }

    public virtual void Reset()
    {
        _stateMachine.GoToState(IInteractable.State.READY);
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
        _stateMachine.GoToState(IInteractable.State.TARGET);
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
        switch (CurrentState)
        {
            case IInteractable.State.START:
            case IInteractable.State.CONTINUE:
                _stateMachine.GoToState(IInteractable.State.CONTINUE);
                break;
            case IInteractable.State.COMPLETE:
            case IInteractable.State.DISABLED:
                Debug.LogError($"{PREFIX} {Name} :: Cannot interact in state: {CurrentState}");
                break;
            default:
                _stateMachine.GoToState(IInteractable.State.START);
                break;
        }

        return true;
    }


    #endregion

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Interactable))]
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

            // << INITIALIZE BUTTON >> ------------------------------------
            if (_script._isInitialized == false
                && GUILayout.Button("Initialize"))
            {
                _script.Initialize();
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


