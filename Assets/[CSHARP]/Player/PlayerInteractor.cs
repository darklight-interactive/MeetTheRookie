using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using Darklight.UnityExt.Inky;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : OverlapGrid2D
{
    public PlayerController playerController => GetComponent<PlayerController>();
    [SerializeField, ShowOnly] protected List<Interactable> _foundInteractables = new List<Interactable>();

    [ShowOnly, Tooltip("The Interactable that the player is currently targeting.")]
    public Interactable targetInteractable;

    [ShowOnly, Tooltip("The Interactable that the player is currently interacting with. Can be null.")]
    public Interactable activeInteractable;

    #region -- [[ UPDATE THE INTERACTABLE RADAR ]] ------------------------------------- >> 

    public override void Update()
    {
        base.Update();
        RefreshRadar();
    }

    void RefreshRadar()
    {
        if (_foundInteractables.Count == 0) return;

        // Temporary list to hold items to be removed
        List<Interactable> toRemove = new List<Interactable>();

        // Iterate through the found interactables
        foreach (Interactable interactable in _foundInteractables)
        {
            if (interactable == null) continue;
            if (interactable.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interactable);
                interactable.TargetClear();
            }
        }

        // Remove the completed interactions from the HashSet
        foreach (Interactable completedInteraction in toRemove)
        {
            _foundInteractables.Remove(completedInteraction);
        }

        // Get the Target Interactable 
        // TODO :: Find the best interactable
        if (targetInteractable == null && _foundInteractables.Count > 0)
        {
            targetInteractable = _foundInteractables.First();
        }

        // Only set the target if the interactable is not the active target
        if (targetInteractable != activeInteractable)
        {
            targetInteractable.TargetSet();
        }
        else if (targetInteractable != null)
        {
            targetInteractable.TargetClear();
        }
    }

    #endregion

    public bool InteractWithTarget()
    {
        if (_foundInteractables.Count == 0) return false;
        if (targetInteractable == null) return false;
        if (targetInteractable.isComplete) return false;

        targetInteractable.TargetClear();

        // If first interaction
        if (activeInteractable != targetInteractable)
        {
            // Set the active interactable
            activeInteractable = targetInteractable;

            // Subscribe to the OnComplete event
            activeInteractable.OnCompleted += ExitInteraction;

            // Set the player controller state to Interaction
            playerController.EnterInteraction();
        }

        activeInteractable.Interact(); // << MAIN INTERACTION

        return true;
    }

    void ExitInteraction()
    {
        Debug.Log("Player Interactor :: Exit Interaction");

        // Clean up
        MTR_UIManager.Instance.DestroySpeechBubble();
        playerController.ExitInteraction();

        // Force set the speaker to Lupe
        InkyStoryManager.Instance.SetSpeaker("Lupe");

        // Unsubscribe from the OnComplete event
        activeInteractable.OnCompleted -= ExitInteraction;
        targetInteractable = null;
        activeInteractable = null;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        Debug.Log($"Player Interactor :: {interactable}");

        if (interactable == null) return;
        if (interactable.isComplete) return;
        _foundInteractables.Add(interactable);

        // Set as target
        targetInteractable = interactable;
        interactable.TargetSet();
    }


    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        _foundInteractables.Remove(interactable);
        interactable.TargetClear();
    }
}
