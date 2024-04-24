using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static Darklight.UnityExt.CustomInspectorGUI;

[RequireComponent(
    typeof(BoxCollider2D),
    typeof(PlayerController),
    typeof(PlayerDialogueHandler))]
public class PlayerInteractor : MonoBehaviour
{
    protected HashSet<Interactable> interactables = new HashSet<Interactable>();
    [SerializeField, ShowOnly] List<InkyInteractable> inkyInteractables = new List<InkyInteractable>();
    [SerializeField, ShowOnly] InkyInteractable activeInkyInteraction;
    [ShowOnly] int interactionCount;

    void Update()
    {
        HandleInteractions();

        interactionCount = interactables.Count;
    }
    void HandleInteractions()
    {
        // Temporary list to hold items to be removed
        List<Interactable> toRemove = new List<Interactable>();

        foreach (Interactable interaction in interactables)
        {
            if (interaction.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interaction);
                interaction.TargetDisable();
            }
            else if (interaction is InkyInteractable)
            {
                if (activeInkyInteraction == null)
                    activeInkyInteraction = interaction as InkyInteractable;

                /*
                Because this method is called every frame, 
                this line will keep the active interaction target at the correct position */
                activeInkyInteraction.TargetEnable();
            }
        }
    }

    public void InteractWithFirstTarget()
    {
        InteractWith(activeInkyInteraction);
    }

    public void InteractWith(InkyInteractable interactable)
    {
        if (interactable == null || interactable.isComplete) return;
        if (interactable is InkyInteractable)
        {
            PlayerDialogueHandler playerDialogueHandler = GetComponent<PlayerDialogueHandler>();
            playerDialogueHandler.HideDialogueBubble();

            if (activeInkyInteraction == interactable || activeInkyInteraction == null)
            {
                // Set as active interaction
                activeInkyInteraction = interactable;
                activeInkyInteraction.TargetDisable();
                activeInkyInteraction.OnCompleted += () =>
                {
                    PlayerController playerController = GetComponent<PlayerController>();
                    playerController.stateMachine.ChangeState(PlayerState.IDLE); // Return to Idle State & reset
                };
            }

            // Start the interaction
            activeInkyInteraction.Interact();

            if (activeInkyInteraction.tempType == TempType.BASE)
            {
                if (activeInkyInteraction.knotIterator.CurrentState == InkyKnotIterator.State.DIALOGUE)
                {
                    playerDialogueHandler.CreateDialogueBubble(activeInkyInteraction.knotIterator.currentText);
                }
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        if (interactable.isComplete) return;
        interactables.Add(interactable);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        interactables.Remove(interactable);

        if (activeInkyInteraction == interactable)
        {
            activeInkyInteraction.TargetDisable();
            activeInkyInteraction = null;
        }

    }

    #region ===== [[ INTERACTION HANDLING ]] ===== >>

    /*
    public class InteractableRadar<Interactable>
    {
        public HashSet<Interactable> interactables = new HashSet<Interactable>();

        public void Add(Interactable interactable)
        {
            interactables.Add(interactable);
        }

        public void Remove(Interactable interactable)
        {
            interactables.Remove(interactable);
        }

        public void Clear()
        {
            interactables.Clear();
        }

        public void HandleInteractions()
        {
            // Temporary list to hold items to be removed
            List<Interactable> toRemove = new List<Interactable>();

            foreach (Interactable interaction in interactables)
            {
                if (interaction.isComplete)
                {
                    // Mark the interaction for removal
                    toRemove.Add(interaction);
                    interaction.TargetDisable();
                }
                else
                {
                    // Optionally, handle active interactions differently if needed
                    interaction.TargetEnable();
                }
            }

            // Remove the completed interactions from the HashSet
            foreach (var completedInteraction in toRemove)
            {
                interactables.Remove(completedInteraction);
            }
        }
    }
    */




    #endregion
}
