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
    protected HashSet<IInteract> interactables = new HashSet<IInteract>();
    [SerializeField, ShowOnly] IInteract _activeInteraction;
    [SerializeField, ShowOnly] int _interactionCount;

    void Update()
    {
        HandleInteractions();
    }
    void HandleInteractions()
    {
        // Temporary list to hold items to be removed
        List<IInteract> toRemove = new List<IInteract>();

        foreach (IInteract interaction in interactables)
        {
            if (interaction.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interaction);
                IInteract interactable = interaction as IInteract;
                interactable.TargetDisable();
            }
            else if (interaction is NPC_Interactable)
            {
                if (_activeInteraction == null)
                    _activeInteraction = interaction as NPC_Interactable;

                /*
                Because this method is called every frame, 
                this line will keep the active interaction target at the correct position */
                IInteract interactable = interaction as IInteract;
                interactable.TargetEnable();
            }
        }

        // Update the interaction count
        _interactionCount = interactables.Count;
    }

    public bool InteractWithActiveTarget()
    {
        return InteractWith(_activeInteraction);
    }

    bool InteractWith(IInteract interactable)
    {
        if (interactable == null || interactable.isComplete) return false;
        if (interactable is NPC_Interactable)
        {
            PlayerDialogueHandler playerDialogueHandler = GetComponent<PlayerDialogueHandler>();
            playerDialogueHandler.HideDialogueBubble();

            if (_activeInteraction == interactable || _activeInteraction == null)
            {
                // Set as active interaction
                _activeInteraction = interactable;
                _activeInteraction.TargetDisable();
                _activeInteraction.OnCompleted += () =>
                {
                    PlayerController playerController = GetComponent<PlayerController>();
                    playerController.stateMachine.ChangeState(PlayerState.IDLE); // Return to Idle State & reset
                };
            }

            // Start the interaction
            _activeInteraction.Interact();

            /*
            if (_activeInteraction.tempType == TempType.BASIC)
            {
                if (_activeInteraction.knotIterator.CurrentState == InkyKnotIterator.State.DIALOGUE)
                {
                    playerDialogueHandler.CreateDialogueBubble(_activeInteraction.knotIterator.currentText);
                }
            }
            */
        }
        return true;
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

        if (_activeInteraction == interactable as IInteract)
        {
            _activeInteraction.TargetDisable();
            _activeInteraction = null;
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
