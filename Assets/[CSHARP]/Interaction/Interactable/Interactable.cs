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

    [SerializeField] Sprite _mainSprite;
    [SerializeField, ShowOnly] IconInteractionHandler _iconHandler;

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
    protected StateMachine stateMachine
    {
        get
        {
            if (_stateMachine == null)
                _stateMachine = new StateMachine(this);
            return _stateMachine;
        }
    }

    public string Name => this.gameObject.name;
    public string SceneKnot => _sceneKnot;
    public string InteractionStitch => _interactionStitch;
    public IInteractable.State CurrentState => stateMachine.CurrentState;
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

        // << REGISTER THE INTERACTABLE >> ------------------------------------
        MTR_InteractionManager.RegisterInteractable(this);

        // << GO TO READY STATE >> ------------------------------------
        stateMachine.GoToState(IInteractable.State.READY);
    }

    public virtual bool AcceptTarget(IInteractor interactor)
    {
        if (interactor == null) return false;
        if (CurrentState != IInteractable.State.READY) return false;


        stateMachine.GoToState(IInteractable.State.TARGET);
        return true;
    }

    public virtual bool AcceptInteraction(IInteractor interactor)
    {
        if (interactor == null) return false;
        if (!VALID_INTERACTION_STATES.Contains(CurrentState)) return false;

        _iconHandler?.HideInteractIcon();

        if (CurrentState == IInteractable.State.TARGET)
            stateMachine.GoToState(IInteractable.State.START);
        else if (CurrentState == IInteractable.State.START)
            stateMachine.GoToState(IInteractable.State.CONTINUE);

        return true;
    }

    public void ForceAcceptInteraction()
    {
        _iconHandler?.HideInteractIcon();

        // Go to start if any other state
        if (CurrentState != IInteractable.State.START)
            stateMachine.GoToState(IInteractable.State.START);
        else
            stateMachine.GoToState(IInteractable.State.CONTINUE);

    }

    public virtual void Reset()
    {
        stateMachine.GoToState(IInteractable.State.READY);
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
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _script.Initialize();
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}

