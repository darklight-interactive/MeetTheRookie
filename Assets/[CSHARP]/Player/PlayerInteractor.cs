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
    PlayerDialogueHandler playerDialogueHandler => GetComponent<PlayerDialogueHandler>();
    PlayerController playerController => GetComponent<PlayerController>();
    PlayerStateMachine stateMachine => playerController.stateMachine;


    protected HashSet<IInteract> interactables = new HashSet<IInteract>();
    [SerializeField, ShowOnly] IInteract _activeInteraction;
    [SerializeField, ShowOnly] int _interactionCount;

    public IInteract ActiveInteractable => _activeInteraction;

    void Update()
    {
        RefreshRadar();

        if (_activeInteraction == null && interactables.Count > 0)
        {
            // Because this method is called every frame, this line will keep the target at the correct position
            interactables.First().TargetEnable();
        }
    }

    void RefreshRadar()
    {
        if (interactables.Count == 0) return;

        // Temporary list to hold items to be removed
        List<IInteract> toRemove = new List<IInteract>();

        // Update the interaction count
        _interactionCount = interactables.Count;

        foreach (IInteract interactable in interactables)
        {
            if (interactable == null) continue;
            if (interactable.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interactable);
                interactable.TargetDisable();
            }
        }

        // Remove the completed interactions from the HashSet
        foreach (IInteract completedInteraction in toRemove)
        {
            interactables.Remove(completedInteraction);
        }
    }

    public bool InteractWithTarget()
    {
        if (interactables.Count == 0) return false;

        IInteract targetInteractable = interactables.First();
        if (targetInteractable == null) return false;
        if (targetInteractable.isComplete) return false;

        targetInteractable.TargetDisable();

        _activeInteraction = targetInteractable;

        // If not active, subscribe to the events
        if (!_activeInteraction.isActive)
        {
            stateMachine.ChangeActiveStateTo(PlayerState.INTERACTION); // Set the Player State to Interaction

            // Subscribe to the Interaction Events
            _activeInteraction.OnInteraction += (string text) =>
            {
                if (_activeInteraction is Clue_Interactable)
                    playerDialogueHandler.CreateDialogueBubble(text);
            };

            // Subscribe to the Completion Event
            _activeInteraction.OnCompleted += () =>
            {
                stateMachine.ChangeActiveStateTo(PlayerState.IDLE); // Return to Idle State & reset

                playerDialogueHandler.HideDialogueBubble();
                _activeInteraction = null;
            };
        }

        // Continue the Interaction
        _activeInteraction.Interact();
        return true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable == null) return;
        interactables.Add(interactable);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable == null) return;
        interactables.Remove(interactable);
        interactable.TargetDisable();
    }
}
