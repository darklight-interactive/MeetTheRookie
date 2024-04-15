using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

public class InkyInteractable : Interactable
{
    [SerializeField] private string inkKnotName;
    [ShowOnly] public int counter = 0;
    private InkyKnotIterator knotIterator;

    [ShowOnly] private string currentText;

    public override void Initialize()
    {
        // Do nothing
    }

    public override void Reset()
    {
        // Do nothing
        counter = 0;
    }

    public override void Interact()
    {
        Debug.Log("Interacting with InkyInteractable");

        if (counter == 0)
        {
            StartInteractionKnot(Complete);
        }
        else
        {
            ContinueInteraction();
        }
        counter++;

        base.Interact();
    }
    public virtual void StartInteractionKnot(InkyKnotIterator.KnotComplete onComplete)
    {
        knotIterator = InkyStoryWeaver.Instance.CreateKnotIterator(inkKnotName);
        Debug.Log("Starting Interaction Knot: " + inkKnotName);
    }

    public virtual void ContinueInteraction()
    {
        InkyStoryWeaver.Instance.ContinueStory();
        Debug.Log("Continuing Interaction Knot: " + inkKnotName);
    }

    public virtual void Complete()
    {

    }
}