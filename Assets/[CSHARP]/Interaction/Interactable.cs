using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : OverlapGrid2D, IInteract
{
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
        isComplete = true;
        isActive = false;
        knotIterator = null;

        OnCompleted?.Invoke();
    }
}