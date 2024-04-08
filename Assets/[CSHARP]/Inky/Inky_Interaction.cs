using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inky_StoryManager;

public enum InteractionType
{
    DIALOGUE
}

public class Inky_Interaction : MonoBehaviour
{
    UXML_InteractionUI interactionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    Inky_StoryManager inkStoryManager => Inky_StoryManager.Instance;
    [SerializeField] private string inkKnot;
    [SerializeField] private InteractionType interactionType = InteractionType.DIALOGUE;

    public virtual void DisplayInteractionPrompt(Vector3 worldPosition)
    {
        interactionUI.DisplayInteractPrompt(worldPosition);
    }

    public virtual void StartInteractionKnot(KnotComplete onComplete)
    {
        inkStoryManager.Run(inkKnot, onComplete);
    }

    public virtual void ResetInteraction()
    {
        interactionUI.HideInteractPrompt();

    }

    public void MoveInteract(Vector2 move)
    {
        Inky_StoryManager.Instance.MoveUpdate(move);
    }
}
