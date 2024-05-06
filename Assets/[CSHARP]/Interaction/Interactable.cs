using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine.UIElements;





#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour, IInteract
{
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();
    private List<string> _storyNameKeys;
    private InkyStory _relatedStory;
    private List<string> _interactionKnotKeys;
    private InkyKnotIterator _knotIterator;
    [SerializeField, ShowOnly] private bool _knotFound;

    public InkyKnotIterator KnotIterator
    {
        get
        {
            if (_knotIterator == null)
                _knotIterator = new InkyKnotIterator(_relatedStory, _interactionKey);
            _knotFound = _knotIterator != null;
            return _knotIterator;
        }
        set
        {
            _knotIterator = value;
            _knotFound = _knotIterator != null;
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    [Header("Interaction Settings")]
    [Dropdown("_storyNameKeys")]
    [SerializeField] string sceneNameKey = "default-scene";

    [DropdownAttribute("_interactionKnotKeys")]
    [SerializeField] string _interactionKey = "default-interaction";
    public string interactionKey { get => _interactionKey; set => _interactionKey = value; }
    [SerializeField] Color _defaultColor = Color.white;
    [SerializeField] Color _interactColor = Color.yellow;

    [Header("Components")]
    [ShowOnly, SerializeField] Sprite _sprite;

    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ====== [[ MONOBEHAVIOUR METHODS ]] =========================
    public void Awake()
    {
        Initialize();
    }

    // ====== [[ INITIALIZATION ]] ================================
    public virtual void Initialize()
    {
        _storyNameKeys = InkyStoryManager.Instance.storyLoader.NameKeys.ToList();
        _relatedStory = InkyStoryManager.Instance.storyLoader.GetStory(sceneNameKey);
        _interactionKnotKeys = _relatedStory.knotAndStitchKeys;
        _spriteRenderer.sprite = _sprite;
        KnotIterator = new InkyKnotIterator(_relatedStory, _interactionKey);
    }

    public virtual void Reset()
    {
        isComplete = false;
        _spriteRenderer.color = _interactColor;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        UIManager.Instance.ShowInteractionPromptInWorld(transform.position);
    }

    public virtual void TargetClear()
    {
        isTarget = false;
        UIManager.Instance.HideInteractPrompt();
    }

    public InkyKnotIterator knotIterator;
    public virtual void Interact()
    {
        // >> TEMPORARY COLOR CHANGE
        StartCoroutine(ColorChangeRoutine(_interactColor, 0.25f));

        // >> CONTINUE KNOT
        //if (knotIterator == null)
        //knotIterator = new InkyKnotIterator(InkyStoryManager.Instance._story, _interactionKey);
        ContinueKnot();
    }

    public virtual void ContinueKnot()
    {
        isTarget = false;
        isActive = true;
        isComplete = false;

        knotIterator.ContinueKnot();

        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
            return;
        }

        OnInteraction?.Invoke(knotIterator.currentText);
    }

    public virtual void Complete()
    {
        isComplete = true;
        isActive = false;
        knotIterator = null;

        OnCompleted?.Invoke();
    }

    public virtual void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable), true)]
public class InteractableCustomEditor : Editor
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

        if (!_script.isTarget && GUILayout.Button("Target"))
        {
            _script.TargetSet();
        }

        if (_script.isTarget && GUILayout.Button("Clear Target"))
        {
            _script.TargetClear();
        }


        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif