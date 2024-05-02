
#region  ABSTRACT INTERACTABLE CLASS ================== //
using Darklight.Game.Grid;
using UnityEngine;
using System.Collections;
using static Darklight.UnityExt.CustomInspectorGUI;

<<<<<<< HEAD

#if UNITY_EDITOR
using UnityEditor;
#endif



public interface IInteract
{
    /// <summary>
    /// Called when the player is targeting the interactable object.
    /// </summary>
    void TargetEnable();

    /// <summary>
    /// Called to disable the interactable object and hide any prompts.
    /// </summary>
    void TargetDisable();

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Reset the interactable object to its default state.
    /// </summary>
    void Reset();

    delegate void OnInteract();
    delegate void OnComplete();
}

=======
>>>>>>> 940056fcd0b6b156086ee58ac2941b378d26f338
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : OverlapGrid2D, IInteract
{
<<<<<<< HEAD
    [ShowOnly] public bool isActive = false;
    [ShowOnly] public bool isComplete = false;
    [SerializeField] private Transform promptIconTarget;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color defaultColor;
=======
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
>>>>>>> 940056fcd0b6b156086ee58ac2941b378d26f338
    public event IInteract.OnInteract OnInteraction;

    public event IInteract.OnComplete OnCompleted;

<<<<<<< HEAD
    // ====== [[ INITIALIZATION ]] ================================
    protected abstract void Initialize();

    public virtual void Reset()
    {
        isComplete = false;
        spriteRenderer.color = defaultColor;
    }

    // ====== [[ TARGETING ]] ======================================

=======
>>>>>>> 940056fcd0b6b156086ee58ac2941b378d26f338
    public virtual void TargetEnable()
    {
        isTarget = true;

        OverlapGrid2D_Data data = this.GetBestData();
        if (data == null) return;
        UIManager.InteractionUI.DisplayInteractPrompt(data.worldPosition);
    }

    public virtual void TargetDisable()
    {
        isTarget = false;
        UIManager.InteractionUI.HideInteractPrompt();
    }

    public InkyKnotIterator knotIterator;
    public virtual void Interact()
    {
<<<<<<< HEAD
        OnInteraction?.Invoke();
        StartCoroutine(ColorChangeRoutine(Color.red,2.0f));
=======
        if (knotIterator == null)
        {
            knotIterator = new InkyKnotIterator(InkyStoryManager.Instance.currentStory, _interactionKey);
        }

        ContinueKnot();
>>>>>>> 940056fcd0b6b156086ee58ac2941b378d26f338
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
<<<<<<< HEAD
        Initialize();
        defaultColor = spriteRenderer.color;
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
=======
        isComplete = true;
        isActive = false;
        knotIterator = null;

        OnCompleted?.Invoke();
    }
}
#endregion
>>>>>>> 940056fcd0b6b156086ee58ac2941b378d26f338
