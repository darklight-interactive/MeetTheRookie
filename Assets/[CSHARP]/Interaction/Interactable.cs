
#region  ABSTRACT INTERACTABLE CLASS ================== //
using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour, IInteract
{
    // << SERIALIZED VALUES >> //
    [ShowOnly, SerializeField] bool _isTargeted;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;
    [SerializeField] string _interactionKey;
    [SerializeField] Transform _promptTarget;

    // << PUBLIC ACCESSORS >> //
    public bool isTargeted { get => _isTargeted; set => _isTargeted = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public string interactionKey { get => _interactionKey; set => _interactionKey = value; }
    public Transform promptTarget
    {
        get
        {
            if (_promptTarget == null)
            {
                Transform promptTarget = new GameObject("prompt target").transform;
                promptTarget.SetParent(this.transform);
                _promptTarget = promptTarget;
            }
            return _promptTarget;
        }
        set => _promptTarget = value;
    }

    // << EVENTS >> //
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // << INTERACTION METHODS >> //
    public virtual void Interact()
    {
        OnInteraction?.Invoke();

        knotIterator?.ContinueKnot();

        // State Handler
        switch (knotIterator.CurrentState)
        {
            case InkyKnotIterator.State.START:
            case InkyKnotIterator.State.DIALOGUE:
                knotIterator.ContinueKnot();
                break;

            case InkyKnotIterator.State.CHOICE:
                // TODO : Implement choice selection using input
                knotIterator.ChooseChoice(0);
                break;
            case InkyKnotIterator.State.END:
                Complete();
                break;
            default:
                break;
        }
    }
    public virtual void Complete()
    {
        OnCompleted?.Invoke();
    }
    // ============================================================================== >>>>


    // << INKY KNOT ITERATOR >> //
    public InkyKnotIterator knotIterator;


    // << MONOBEHAVIOUR METHODS >> //



    public void Update()
    {
        if (isComplete || knotIterator == null) return;

        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
        }
    }
}
#endregion
