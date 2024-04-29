using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

public enum TempType { BASIC, NPC }

public class InkyInteractable : Interactable
{
    [SerializeField] public TempType tempType;
    [SerializeField] private string inkKnotName;
    public InkyKnotIterator knotIterator;

    [ShowOnly] public string currentText;
    [ShowOnly] public InkyKnotIterator.State currentKnotState = InkyKnotIterator.State.NULL;

    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    public void Update()
    {
        if (isComplete || knotIterator == null) return;
        currentKnotState = knotIterator.CurrentState;
        currentText = knotIterator.currentText;
        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
        }
    }


    public override void Interact()
    {
        //Debug.Log("Interacting with InkyInteractable");

        // Move the story to this knot
        if (knotIterator == null)
        {
            knotIterator = InkyStoryManager.Instance.CreateKnotIterator(inkKnotName);
        }

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

        if (knotIterator.CurrentState != InkyKnotIterator.State.END
        && knotIterator.CurrentState != InkyKnotIterator.State.NULL)
        {
            // Call the base interact method to invoke the OnInteraction event
            base.Interact();
        }
    }

    public override void Complete()
    {
        base.Complete();// Invoke the OnInteractionCompleted event
        Debug.Log("Completing Interaction Knot: " + inkKnotName);
    }

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }
}