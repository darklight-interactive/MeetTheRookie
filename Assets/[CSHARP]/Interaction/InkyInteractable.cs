using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

public class InkyInteractable : Interactable
{
    [SerializeField] private string inkKnotName;
    public InkyKnotIterator knotIterator;

    [ShowOnly] private string currentText;

    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }


    public override void Interact()
    {
        Debug.Log("Interacting with InkyInteractable");

        // Move the story to this knot
        if (knotIterator == null)
        {
            knotIterator = InkyStoryWeaver.Instance.CreateKnotIterator(inkKnotName);
        }

        // State Handler
        switch (knotIterator.CurrentState)
        {
            case InkyKnotIterator.State.START:
            case InkyKnotIterator.State.DIALOGUE:
                knotIterator.ContinueKnot();
                currentText = knotIterator.currentText;
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

        // Call the base interact method to invoke the OnInteraction event
        if (knotIterator.CurrentState != InkyKnotIterator.State.END
        && knotIterator.CurrentState != InkyKnotIterator.State.NULL)
        {
            base.Interact();
        }
    }

    public override void Complete()
    {
        base.Complete();
        Debug.Log("Completing Interaction Knot: " + inkKnotName); // Invoke the OnInteractionCompleted event
    }

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }
}