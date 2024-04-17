using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static Darklight.UnityExt.CustomInspectorGUI;

[RequireComponent(typeof(BoxCollider2D), typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    PlayerController playerController => GetComponent<PlayerController>();
    PlayerDialogueHandler playerDialogueHandler => GetComponent<PlayerDialogueHandler>();


    protected HashSet<Interactable> interactions = new HashSet<Interactable>();

    [SerializeField] private List<Interactable> interactables;

    [ShowOnly] int interactionCount;
    [ShowOnly] Interactable targetInteraction;
    [ShowOnly] Interactable activeInteraction;

    void Update()
    {
        HandleInteractions();

        interactionCount = interactions.Count;
        interactables = interactions.ToList();
    }

    public void InteractWithFirstTarget()
    {
        InteractWith(targetInteraction);
    }

    public void InteractWith(Interactable interactable)
    {
        if (interactable == null) return;
        if (interactable.isComplete) return;

        // Set as active interaction
        activeInteraction = interactable;

        // Start the interaction
        activeInteraction.Interact();

        // Lupe's Dialogue
        if (interactable is InkyInteractable)
        {
            InkyInteractable inkyInteractable = interactable as InkyInteractable;
            playerDialogueHandler.CreateDialogueBubble(inkyInteractable.knotIterator.currentText);
        }
    }

    #region ===== [[ INTERACTION HANDLING ]] ===== >>

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        if (interactable.isComplete) return;

        interactable.TargetEnable();

        Debug.Log("Interactable found: " + other.name);
        interactions.Add(interactable);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;

        interactable.TargetDisable();
        interactions.Remove(interactable);
    }

    void HandleInteractions()
    {
        if (interactions.Count > 0)
        {
            Interactable firstInteraction = interactions.First();
            if (firstInteraction.isComplete)
            {
                // Remove the interaction & disable the target
                interactions.Remove(firstInteraction);
                firstInteraction.TargetDisable();
            }
            else
            {
                // Set the target interaction
                targetInteraction = firstInteraction;
                this.targetInteraction.TargetEnable();
            }
        }
    }


    #endregion
}
