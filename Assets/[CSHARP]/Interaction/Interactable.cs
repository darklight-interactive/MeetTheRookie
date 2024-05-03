using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : OverlapGrid2D, IInteract
{
    [SerializeField] private Transform promptIconTarget;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    // << SERIALIZED VALUES >> //
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;
    [SerializeField] string _interactionKey;

    // << PUBLIC ACCESSORS >> //
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public string interactionKey { get => _interactionKey; set => _interactionKey = value; }

    // << EVENTS >> //
    public event IInteract.OnInteract OnInteraction;

    public event IInteract.OnComplete OnCompleted;

    // ====== [[ INITIALIZATION ]] ================================
    protected abstract void Initialize();

    public virtual void Reset()
    {
        isComplete = false;
        spriteRenderer.color = defaultColor;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetEnable()
    {
        isTarget = true;

        OverlapGrid2D_Data data = this.GetBestData();
        if (data == null) return;
        UIManager.Instance.interactionUI.DisplayInteractPrompt(data.worldPosition);
    }

    public virtual void TargetDisable()
    {
        isTarget = false;
        UIManager.Instance.interactionUI.HideInteractPrompt();
    }

    public InkyKnotIterator knotIterator;
    public virtual void Interact()
    {
        StartCoroutine(ColorChangeRoutine(Color.red, 2.0f));
        if (knotIterator == null)
        {
            knotIterator = new InkyKnotIterator(InkyStoryManager.Instance.currentStory, _interactionKey);
        }

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
        Initialize();
        defaultColor = spriteRenderer.color;

        isComplete = true;
        isActive = false;
        knotIterator = null;

        OnCompleted?.Invoke();
    }

    public abstract void OnDestroy();

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }
}