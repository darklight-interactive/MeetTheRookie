using UnityEngine;

public class InkyInteractable : Interactable
{
    [SerializeField] private string inkKnot;
    private int counter = 0;

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
        counter++;
        if (counter == 1)
        {
            StartInteractionKnot(OnInteractionComplete);
        }
        else
        {
            ContinueInteraction();
        }
    }
    public virtual void StartInteractionKnot(INKY_KnotIterator.KnotComplete onComplete)
    {
        InkyStoryWeaver.Instance.GoToKnotAt(inkKnot);
    }

    public virtual void ContinueInteraction()
    {
        InkyStoryWeaver.Instance.ContinueStory();
    }

    public virtual void OnInteractionComplete()
    {
        Reset();
    }
}