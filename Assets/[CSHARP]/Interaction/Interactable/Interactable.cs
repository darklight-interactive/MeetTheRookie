using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public partial class Interactable : MonoBehaviour, IInteractable
{
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
    Collider2D _collider;
    StateMachine _stateMachine;

    [SerializeField, ShowOnly] IInteractable.State _currentState;
    [SerializeField, ShowOnly] IconInteractionHandler _iconHandler;
    [SerializeField] Sprite _mainSprite;


    [Header("InkyStory")]
    [DropdownAttribute("_sceneKnots")]
    string _sceneKnot = "scene1_0";

    [DropdownAttribute("_interactionStitches")]
    string _interactionStitch;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    #region ======== [[ PROPERTIES ]] ================================== >>>>
    // private access to knots for dropdown
    List<string> _sceneKnots
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }

    // private access to stitches for dropdown
    List<string> _interactionStitches
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllStitchesInKnot(storyObject.StoryValue, _sceneKnot);
        }
    }

    InkyStoryIterator storyIterator => InkyStoryManager.Iterator;
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
    public IconInteractionHandler IconHandler
    {
        get
        {
            if (_iconHandler == null)
                _iconHandler = GetComponentInChildren<IconInteractionHandler>();
            return _iconHandler;
        }
        set => _iconHandler = value;
    }
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

    public virtual void Initialize()
    {
        // << GET THE SPRITE RENDERER >> ------------------------------------
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        // << SET THE INITIAL SPRITE >> ------------------------------------
        if (_mainSprite != null)
            _spriteRenderer.sprite = _mainSprite;
        else if (_spriteRenderer.sprite != null)
            _mainSprite = _spriteRenderer.sprite;

        // Set the default tint
        _spriteRenderer.color = _defaultTint;

        // << GET THE COLLIDER >> ------------------------------------
        _collider = GetComponent<Collider2D>();
        if (_collider == null)
            _collider = this.gameObject.AddComponent<BoxCollider2D>();

        // << CREATE THE STATE MACHINE >> ------------------------------------
        _stateMachine = new StateMachine(this);

        // << SUBSCRIBE TO STATE CHANGES >> ------------------------------------
        _stateMachine.OnStateChanged += (IInteractable.State state) =>
        {
            _currentState = state;
            //Debug.Log($"[{Name}] State Changed: {state}");
        };

        // << REGISTER THE INTERACTABLE >> ------------------------------------
        MTR_InteractionManager.RegisterInteractable(this);

        // << GO TO READY STATE >> ------------------------------------
        _stateMachine.GoToState(IInteractable.State.READY);

        Debug.Log($"<INTERACTABLE> {Name} Initialized. Current State: {CurrentState}");
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

