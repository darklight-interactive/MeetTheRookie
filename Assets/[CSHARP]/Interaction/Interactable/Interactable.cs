using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public partial class Interactable : MonoBehaviour,
    IInteractable, IUnityEditorListener
{
    const string PREFIX = "<INTERACTABLE>";

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

    protected List<string> dropdown_sceneKnotList
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }

    List<string> dropdown_interactionStitchList
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllStitchesInKnot(storyObject.StoryValue, _sceneKnot);
        }
    }
    #endregion

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider;
    StateMachine _stateMachine;

    [SerializeField, ShowOnly] bool _isPreloaded = false;
    [SerializeField, ShowOnly] bool _isInitialized = false;

    [Header(" (( INTERACTABLE VALUES )) -------- >>")]
    [SerializeField, ShowOnly] IInteractable.State _currentState;
    [SerializeField] Sprite _mainSprite;

    [Header(" (( INKY STORY KEYS )) -------- >>")]
    [Dropdown("dropdown_sceneKnotList")]
    [SerializeField] string _sceneKnot = "scene1_0";

    [Dropdown("dropdown_interactionStitchList")]
    [SerializeField] string _interactionStitch;

    [Header(" (( INTERACTION SETTINGS )) -------- >>")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    [Header(" (( INTERACTION HANDLERS )) -------- >>")]
    [SerializeField, ShowOnly] IconInteractionHandler _iconHandler;


    #region ======== [[ PROPERTIES ]] ================================== >>>>
    public string Name => this.gameObject.name;
    public string SceneKnot => _sceneKnot;
    public string InteractionStitch => _interactionStitch;
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
    public Sprite MainSprite { get => _mainSprite; set => _mainSprite = value; }
    public bool IsPreloaded => _isPreloaded;
    public bool IsInitialized => _isInitialized;


    public IconInteractionHandler IconHandler { get => _iconHandler; set => _iconHandler = value; }

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
        if (_mainSprite != null)
            _spriteRenderer.sprite = _mainSprite;
        else if (_spriteRenderer.sprite != null)
            _mainSprite = _spriteRenderer.sprite;

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


    #endregion

    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
    public virtual void Preload()
    {
        PreloadSpriteRenderer();
        PreloadBoxCollider();

        // << REGISTER THE INTERACTABLE >> ------------------------------------
        InteractionRegistry.RegisterInteractable(this);

        _isPreloaded = true;
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
        if (CurrentState == IInteractable.State.START)
            _stateMachine.GoToState(IInteractable.State.CONTINUE);
        else
            _stateMachine.GoToState(IInteractable.State.START);
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

}

