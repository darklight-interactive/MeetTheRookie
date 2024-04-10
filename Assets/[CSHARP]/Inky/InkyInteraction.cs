using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    DIALOGUE
}

public class InkyInteraction : MonoBehaviour
{
    UXML_InteractionUI interactionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    [SerializeField] private string inkKnot;
    [SerializeField] private InteractionType interactionType = InteractionType.DIALOGUE;

    public virtual void DisplayInteractionPrompt(Vector3 worldPosition)
    {
        interactionUI.DisplayInteractPrompt(worldPosition);
    }

    public virtual void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        InkyKnotThreader.Instance.GoToKnotAt(inkKnot);
        InkyKnotThreader.Instance.ContinueStory();
    }

    public virtual void ContinueDialogue()
    {
        InkyKnotThreader.Instance.ContinueStory();
    }

    public virtual void ResetInteraction()
    {
        interactionUI.HideInteractPrompt();
    }

    public void MoveInteract(Vector2 move)
    {
        //I.MoveUpdate(move);
    }
}
