using System.Collections;
using System.Collections.Generic;

using Darklight.Game.Grid;
using Darklight.UnityExt.Audio;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;

using FMODUnity;

using NaughtyAttributes;

using UnityEngine;
using Ink.Runtime;


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
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }

    // private access to stitches for dropdown
    private List<string> _interactionStitches
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllStitchesInKnot(storyObject.StoryValue, _sceneKnot);
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    //[HorizontalLine(color: EColor.Gray)]
    [Header("Interactable")]
    [SerializeField, ShowAssetPreview] Sprite _sprite;
    [SerializeField] private bool onStart;

    [Header("InkyStory")]
    [Tooltip("The parent InkyStoryObject that this interactable belongs to. This is equivalent to a 'Level' of the game.")]

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
    public string interactionKey { get => _interactionStitch; private set => _interactionStitch = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }

    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ------------------- [[ PUBLIC METHODS ]] ------------------- >>

    public virtual void Start()
    {
        // << SET THE INITIAL SPRITE >> ------------------------------------
        // Prioritize the initial sprite that is set in the sprite renderer
        // Its assumed that the sprite renderer has a null sprite when the interactable is first created
        if (_spriteRenderer.sprite == null)
            _spriteRenderer.sprite = _sprite;
        else
            _sprite = _spriteRenderer.sprite;
        _spriteRenderer.color = _defaultTint;

        Invoke(nameof(OnStart), 0.1f);
    }

    void OnStart()
    {
        if (onStart)
        {
            this.Reset();

            PlayerInteractor playerInteractor = FindFirstObjectByType<PlayerInteractor>();
            playerInteractor.ForceInteract(this);
        }
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
        else if (StoryIterator.CurrentState == InkyStoryIterator.State.CHOICE)
        {
            List<Choice> choices = StoryIterator.GetCurrentChoices();
            MTR_UIManager.Instance.gameUIController.LoadChoices(choices);
            Debug.Log($"INTERACTABLE :: {name} >> Choices Found");
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