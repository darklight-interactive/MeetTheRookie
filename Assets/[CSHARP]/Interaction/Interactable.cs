using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine.UIElements;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : OverlapGrid2D, IInteract
{
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();
    private InkyStoryIterator _iterator;
    private List<string> _sceneKnots
    {
        get
        {
            if (_storyObject == null) return new List<string>();
            return _storyObject.GetKnots();
        }
    }
    private List<string> _interactionStitches
    {
        get
        {
            if (_storyObject == null) return new List<string>();
            if (_sceneKnot == null || _sceneKnot == "") return new List<string>();
            return _storyObject.GetStitches(_sceneKnot);
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    [HorizontalLine(color: EColor.Gray)]
    [Header("Interactable")]
    [SerializeField, ShowAssetPreview] Sprite _sprite;

    [Tooltip("The parent InkyStoryObject that this interactable belongs to. This is equivalent to a 'Level' of the game.")]
    [SerializeField] protected InkyStoryObject _storyObject;

    [Dropdown("_sceneKnots")]
    [SerializeField] protected string _sceneKnot;

    [Dropdown("_interactionStitches")]
    [SerializeField] protected string _interactionStitch;


    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    public InkyStoryObject storyObject { get => _storyObject; private set => _storyObject = value; }
    public string interactionKey { get => _interactionStitch; private set => _interactionStitch = value; }
    public InkyStoryIterator knotIterator { get => _iterator; private set => _iterator = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ====== [[ MONOBEHAVIOUR METHODS ]] =========================
    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    // ====== [[ INITIALIZATION ]] ================================
    public virtual void Initialize()
    {
        _spriteRenderer.sprite = _sprite;
        _spriteRenderer.color = _defaultTint;

        if (_storyObject == null)
        {
            Debug.LogError("Story Parent is null. Please assign a valid InkyStory object.");
            return;
        }

        if (_interactionStitch == null || _interactionStitch == "")
        {
            Debug.LogError("Interaction Key is null. Please assign a valid knot or stitch key.");
            return;
        }

        _iterator = new InkyStoryIterator(storyObject, InkyStoryIterator.State.NULL);
    }

    public virtual void Reset()
    {
        isComplete = false;
        _spriteRenderer.color = _interactionTint;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        UIManager.Instance.gameUIController.ShowInteractIcon(transform.position);
    }

    public virtual void TargetClear()
    {
        isTarget = false;
        UIManager.Instance.gameUIController.HideInteractIcon();
    }

    public virtual void Interact()
    {
        // >> TEMPORARY COLOR CHANGE
        StartCoroutine(ColorChangeRoutine(_interactionTint, 0.25f));

        // >> CONTINUE KNOT
        ContinueKnot();
    }

    public virtual void ContinueKnot()
    {
        if (!isActive)
        {
            OnFirstInteraction?.Invoke();
        }

        isTarget = false;
        isActive = true;
        isComplete = false;

        knotIterator.ContinueKnot();

        if (knotIterator.CurrentState == InkyStoryIterator.State.END)
        {
            Complete();
            return;
        }

        OnInteraction?.Invoke(knotIterator.CurrentStoryTest);
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
public class InteractableCustomEditor : OverlapGrid2DEditor
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



        GUILayout.Space(10);
        GUILayout.Label("Interactable Testing", EditorStyles.boldLabel);
        if (!_script.isTarget && GUILayout.Button("Target"))
        {
            _script.TargetSet();
        }

        if (_script.isTarget && GUILayout.Button("Clear Target"))
        {
            _script.TargetClear();
        }

        EditorGUILayout.Space();

        // Draw the default inspector
        //CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(serializedObject);
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif