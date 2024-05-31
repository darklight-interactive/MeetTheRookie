using System.Collections;
using System.Collections.Generic;

using Darklight.Game.Grid;
using Darklight.UnityExt.Audio;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;

using FMODUnity;

using NaughtyAttributes;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : OverlapGrid2D, IInteract
{
    private const string Prefix = "[Interactable] >> ";
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();

    // private access to knots for dropdown
    private List<string> _sceneKnots
    {
        get
        {
            InkyStoryManager storyManager = InkyStoryManager.Instance;
            if (storyManager == null) return new List<string>();
            return InkyStoryManager.GlobalStoryObject.KnotNames;
        }
    }

    // private access to stitches for dropdown
    private List<string> _interactionStitches
    {
        get
        {
            if (_storyObject == null) return new List<string>();
            if (_sceneKnot == null || _sceneKnot == "") return new List<string>();
            return InkyStoryObject.GetAllStitchesInKnot(_storyObject.StoryValue, _sceneKnot);
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    //[HorizontalLine(color: EColor.Gray)]
    [Header("Interactable")]
    [SerializeField, ShowAssetPreview] Sprite _sprite;

    [Header("InkyStory")]
    [Tooltip("The parent InkyStoryObject that this interactable belongs to. This is equivalent to a 'Level' of the game.")]
    protected InkyStoryObject _storyObject;

    [DropdownAttribute("_sceneKnots")]
    public string _sceneKnot;

    [DropdownAttribute("_interactionStitches")]
    public string _interactionStitch;

    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive; [ShowOnly, SerializeField] bool _isComplete;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    [Header("Outline")]
    [SerializeField] Material _outlineMaterial;

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    public InkyStoryObject storyObject { get => _storyObject; private set => _storyObject = value; }
    public string interactionKey { get => _interactionStitch; private set => _interactionStitch = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }

    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ------------------- [[ PUBLIC METHODS ]] ------------------- >>
    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public virtual void Initialize()
    {

        // << SET THE INITIAL SPRITE >> ------------------------------------
        // Prioritize the initial sprite that is set in the sprite renderer
        // Its assumed that the sprite renderer has a null sprite when the interactable is first created
        if (_spriteRenderer.sprite == null)
            _spriteRenderer.sprite = _sprite;
        else
            _sprite = _spriteRenderer.sprite;
        _spriteRenderer.color = _defaultTint;

        // << SET THE STORY OBJECT >> ------------------------------------
        if (_storyObject == null)
        {
            if (InkyStoryManager.Instance == null) { Debug.LogError("Could not find InkyStoryManager"); }
            _storyObject = InkyStoryManager.GlobalStoryObject;
        }

        OnFirstInteraction += () => 
        {
        };
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        OverlapGrid2D_Data targetData = GetBestOverlapGridData();

        if (MTR_UIManager.Instance != null)
            MTR_UIManager.Instance.ShowInteractIcon(transform.position, targetData.cellSize);
    }

    public virtual void TargetClear()
    {
        isTarget = false;

        if (MTR_UIManager.Instance != null)
            MTR_UIManager.Instance.RemoveInteractIcon();
    }

    // ====== [[ INTERACTION ]] ======================================
    public virtual void Interact()
    {
        InkyStoryIterator StoryIterator = InkyStoryManager.Iterator;

        // << FIRST INTERACTION >>
        if (!isActive)
        {
            TargetClear();

            // Set the active flags
            isActive = true;
            isComplete = false;

            // Subscribe to OnInteraction
            OnInteraction += (string text) =>
            {
                MTR_UIManager.Instance.CreateNewSpeechBubble(text);
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();
            };

            // Subscribe to OnComplete
            OnCompleted += () =>
            {
                // Destroy the speech bubble
                MTR_UIManager.Instance.DestroySpeechBubble();
                MTR_AudioManager.Instance.PlayEndInteractionEvent();
            };

            // Go To the Interaction Stitch
            StoryIterator.GoToKnotOrStitch(_interactionStitch);

            // Color Flash
            StartCoroutine(ColorChangeRoutine(_interactionTint, 0.25f));

            OnFirstInteraction?.Invoke();
            MTR_AudioManager.Instance.PlayFirstInteractionEvent();
            Debug.Log($"INTERACTABLE :: {name} >> First Interaction");
        }

        // << CONTINUE INTERACTION >> ------------------------------------
        StoryIterator.ContinueStory();

        // << LAST INTERACTION >> ----------------------------------------
        if (StoryIterator.CurrentState == InkyStoryIterator.State.END)
        {
            Complete();
        }
        else
        {
            OnInteraction?.Invoke(StoryIterator.CurrentText);

            Debug.Log($"INTERACTABLE :: {name} >> Continue Interaction");
        }
    }

    public virtual void Complete()
    {
        OnCompleted?.Invoke(); // Invoke OnCompleted
        Debug.Log($"INTERACTABLE :: {name} >> Complete");

        // Set the flags to complete
        isActive = false;
        isTarget = false;
        isComplete = true;

        // Reset the interactable after 1 second
        Invoke(nameof(Reset), 1.0f);
    }

    /// <summary>
    /// Reset all flags and values
    /// </summary>
    public virtual void Reset()
    {
        isTarget = false;
        isActive = false;
        isComplete = false;

        _spriteRenderer.color = _defaultTint;
    }

    public virtual void OnDestroy()
    {
        OnFirstInteraction = delegate { }; // Reset OnFirstInteraction
        OnInteraction = delegate { }; // Reset OnInteraction
        OnCompleted = delegate { }; // Reset OnCompleted
    }

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }

    private void EnableOutline(bool enable)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.material = enable ? _outlineMaterial : null;
        }
    }

    private IEnumerator FlashOutlineRoutine()
    {
        EnableOutline(true);
        yield return new WaitForSeconds(0.25f);
        EnableOutline(false);
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

        if (!_script.isTarget)
        {
            if (GUILayout.Button("Set Target"))
                _script.TargetSet();

            if (_script.isActive)
            {
                if (GUILayout.Button("Continue Interaction"))
                    _script.Interact();

                if (GUILayout.Button("Complete Interaction"))
                    _script.Complete();

                if (GUILayout.Button("Reset Interaction"))
                    _script.Reset();
            }
        }
        else
        {
            if (GUILayout.Button("Clear Target"))
                _script.TargetClear();

            if (!_script.isActive)
            {
                if (GUILayout.Button("First Interact"))
                    _script.Interact();
            }
        }


        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif