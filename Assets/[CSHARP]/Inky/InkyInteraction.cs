using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InkyStoryContainer;

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

    public virtual void StartInteractionKnot(KnotComplete onComplete)
    {
        //inkStoryManager.Run(inkKnot, onComplete);
    }

    /*
    public virtual InkyDialogue ContinueDialogue()
    {
        return inkStoryManager.Continue();
    }
    */

    public virtual void ResetInteraction()
    {
        interactionUI.HideInteractPrompt();
    }

    public void MoveInteract(Vector2 move)
    {
        //I.MoveUpdate(move);
    }
}
