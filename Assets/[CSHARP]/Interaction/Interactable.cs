using Darklight.Game.Grid;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteractable
{
    int counter { get; set; }
    public abstract void Target();
    public virtual void Interact()
    {
        counter++;
    }
    public virtual void Reset()
    {
        counter = 0;
    }
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected string inkKnotName = "default";
    [SerializeField] protected Transform promptUITarget;

    public string inkKnot => inkKnotName;
    public int counter { get; set; }

    public virtual void Target()
    {
        UXML_InteractionUI.Instance.DisplayInteractPrompt(promptUITarget.position);
    }

    public virtual void Disable()
    {
        UXML_InteractionUI.Instance.HideInteractPrompt();
    }

    public virtual void Interact()
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
    public virtual void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        InkyThreader.Instance.GoToKnotAt(inkKnot);
    }

    public virtual void ContinueInteraction()
    {
        InkyThreader.Instance.ContinueStory();
    }

    public virtual void OnInteractionComplete()
    {
        // Reset the interaction counter
        counter = 0;
    }
}