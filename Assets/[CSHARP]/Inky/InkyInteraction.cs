using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    DIALOGUE
}

public class InkyInteraction : I_Interaction
{
    UXML_InteractionUI interactionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    [SerializeField] private string inkyKnot;
    public override void Interact()
    {
        UXML_InteractionUI.Instance.HideInteractPrompt();

        if (counter == 0)
        {
            StartInteractionKnot(() =>
            {
                //interactionUI.HideInteractPrompt();
            });
        }
        else
        {
            InkyKnotThreader.Instance.ContinueStory();
        }

        base.Interact();
    }

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
