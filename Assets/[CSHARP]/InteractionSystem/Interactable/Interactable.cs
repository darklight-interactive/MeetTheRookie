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




#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public partial class Interactable : MonoBehaviour,
    IInteractable, IUnityEditorListener
{
    public const string PREFIX = "<INT>";
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
    [SerializeField, ShowOnly] string _name = DEFAULT_NAME;
    [SerializeField, ShowOnly] string _key = DEFAULT_KEY;
    [SerializeField, ShowOnly] string _layer = DEFAULT_LAYER;
    [SerializeField, ShowOnly] IInteractable.State _currentState;
    [SerializeField, ShowAssetPreview] Sprite _sprite;

    [Header(" (( INTERACTION HANDLERS )) -------- >>")]
    [SerializeField] InteractionRequestPreset _preset;
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

    #region ---- ( Preload ) ---- ))
    void PreloadRequestPreset()
    {
        if (_preset == null)
        {
            _preset = InteractionSystem.Factory.CreateOrLoadRequestPreset();
        }
    }

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

    void PreloadRecievers()
    {
        RemoveUnusedRecievers();

        _recievers.Reset();

        List<InteractionTypeKey> keys = _recievers.Keys.ToList();
        foreach (InteractionTypeKey key in keys)
        {
            _recievers.TryGetValue(key, out InteractionReciever currentHandlerValue);
            if (currentHandlerValue == null)
            {
                InteractionReciever recieverInChild = GetRecieverInChildren(key);
                if (recieverInChild != null)
                {
                    _recievers[key] = recieverInChild;
                    continue;
                }

                GameObject recieverGameObject = _preset.CreateRecieverGameObject(key);
                if (recieverGameObject == null)
                {
                    Debug.LogError($"CreateInteractionHandler failed for key {key}. GameObject is null.", this);
                    continue;
                }
                else
                {
                    recieverGameObject.transform.SetParent(this.transform);
                    recieverGameObject.transform.localPosition = Vector3.zero;
                    recieverGameObject.transform.localRotation = Quaternion.identity;
                    recieverGameObject.transform.localScale = Vector3.one;
                    recieverGameObject.name = $"{key} Interaction Handler";
                }

                InteractionReciever handler = recieverGameObject.GetComponent<InteractionReciever>();
                if (handler == null)
                {
                    Debug.LogError($"CreateInteractionHandler failed for key {key}. GameObject does not contain InteractionHandler.", this);
                    ObjectUtility.DestroyAlways(recieverGameObject);
                    continue;
                }



                _recievers[key] = handler;
            }
        }

        //Debug.Log($"Preloaded Interaction Handlers for {Name}. Count {_handlerLibrary.Count}", this);
    }

    bool IsPreloadValid(bool logErrors = false)
    {
        bool isValid = _isRegistered
            && _recievers.Count > 0
            && !_recievers.HasUnsetKeysOrValues();

        if (!isValid && logErrors)
        {
            string outLog = $"{PREFIX} {Name} :: Preload Validation Failed";
            if (!_isRegistered)
                outLog += " :: Not Registered";
            if (_recievers.Count == 0)
                outLog += " :: No Recievers Requested";
            if (_recievers.HasUnsetKeysOrValues())
                outLog += " :: Found Unset Recievers";
            Debug.LogError(outLog, this);
        }

        return isValid;
    }


    #endregion ---- ( Preload ) ------------------------------------ >>>>

    InteractionReciever GetRecieverInChildren(InteractionTypeKey key)
    {
        InteractionReciever[] handlers = GetComponentsInChildren<InteractionReciever>();
        foreach (InteractionReciever handler in handlers)
        {
            if (handler.InteractionType == key)
                return handler;
        }
        return null;
    }

    void RemoveUnusedRecievers()
    {
        InteractionReciever[] allRecieversInChildren = GetComponentsInChildren<InteractionReciever>();
        foreach (InteractionReciever childReciever in allRecieversInChildren)
        {
            if (!_recievers.ContainsKey(childReciever.InteractionType)
                || _recievers[childReciever.InteractionType] != null)
            {
                ObjectUtility.DestroyAlways(childReciever.gameObject);
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


        PreloadRequestPreset();
        PreloadSpriteRenderer();
        PreloadBoxCollider();
        PreloadRecievers();

        // << REGISTER THE INTERACTABLE >> ------------------------------------
        _isRegistered = InteractionSystem.Registry.TryRegister(this);

        // << Determine if the Interactable is Preloaded >> ------------------------------------
        _isPreloaded = IsPreloadValid(true);

    }

    public virtual void Initialize()
    {
        if (!_isPreloaded) Preload();

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
        if (_stateMachine == null)
            return;
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
        Debug.Log($"{PREFIX} {Name} :: AcceptInteraction from {interactor}", this);
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
            if (!_script._isPreloaded)
            {
                if (GUILayout.Button("Preload"))
                {
                    _script.Preload();
                }
            }
            else if (!_script._isInitialized)
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


