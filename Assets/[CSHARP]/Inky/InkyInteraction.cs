using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    DIALOGUE
}

public class InkyInteraction : I_Interaction
{
    [SerializeField] private string inkyKnot;
    public virtual void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        InkyKnotThreader.Instance.GoToKnotAt(inkyKnot);
        InkyKnotThreader.Instance.ContinueStory();
    }

    public virtual void ResetInteraction()
    {
        //interactionUI.HideInteractPrompt();
    }

    public void MoveInteract(Vector2 move)
    {
        //I.MoveUpdate(move);
    }
}
