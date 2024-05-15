using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Darklight.UnityExt.Editor;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    public PlayerController playerController => GetComponent<PlayerController>();
    public PlayerDialogueHandler playerDialogueHandler;

    protected HashSet<IInteract> interactables = new HashSet<IInteract>();
    [SerializeField, ShowOnly] IInteract _activeInteraction;
    [SerializeField, ShowOnly] int _interactablesCount;

    public IInteract ActiveInteractable => _activeInteraction;

    void Update()
    {
        RefreshRadar();

        if (interactables.Count == 0) return;
        // Because this method is called every frame, this line will keep the target at the correct position
        interactables.First().TargetSet();
    }

    void RefreshRadar()
    {
        if (interactables.Count == 0) return;

        // Temporary list to hold items to be removed
        List<IInteract> toRemove = new List<IInteract>();

        // Update the interaction count
        _interactablesCount = interactables.Count;

        foreach (IInteract interactable in interactables)
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
        foreach (IInteract completedInteraction in toRemove)
        {
            interactables.Remove(completedInteraction);
        }
    }

    public bool InteractWithTarget()
    {
        if (interactables.Count == 0) return false;

        // Get the Target Interactable
        IInteract targetInteractable = interactables.First();
        if (targetInteractable == null || targetInteractable.isComplete) return false;
        targetInteractable.TargetClear();

        // If the target is not the same as the active interaction, 
        // then set the active interaction to the target and subscribe to the events
        if (_activeInteraction != targetInteractable)
        {
            _activeInteraction = targetInteractable;
            _activeInteraction.OnInteraction += OnInteraction;
            _activeInteraction.OnCompleted += OnComplete;
        }

        // Continue the Interaction
        _activeInteraction.Interact();
        if (_activeInteraction.isComplete)
        {
            _activeInteraction.OnInteraction -= OnInteraction;
            _activeInteraction.OnCompleted -= OnComplete;
            _activeInteraction = null;
            return false;
        }
        return true;
    }

    void OnInteraction(string text)
    {
        if (_activeInteraction is NPC_Interactable)
        {
            NPC_Interactable npcInteractable = _activeInteraction as NPC_Interactable;
            playerController.cameraController.SetOffsetRotation(playerController.transform, npcInteractable.transform);
            //npcInteractable.DialogueBubble.TextureUpdate();
        }

        // Show the player's dialogue bubble
        else if (_activeInteraction is Interactable)
        {
            Interactable interactable = _activeInteraction as Interactable;
            //playerDialogueHandler.Sh
        }

        Debug.Log($"Interacting with {_activeInteraction} => {text}");
    }

    void OnComplete()
    {
        //playerDialogueHandler.HideDialogueBubble();
        playerController.ExitInteraction();
        interactables.Remove(_activeInteraction);
    }

    private void OnDestroy()
    {
        if (_activeInteraction != null)
        {
            _activeInteraction.OnInteraction -= OnInteraction;
            _activeInteraction.OnCompleted -= OnComplete;
        }
    }

    public Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) * 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable == null) return;
        if (interactable.isComplete) return;
        interactables.Add(interactable);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable == null) return;
        interactables.Remove(interactable);
        interactable.TargetClear();
    }
}
