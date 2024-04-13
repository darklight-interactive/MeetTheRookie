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
    public PlayerController playerController => GetComponent<PlayerController>();
    protected HashSet<IInteract> interactions = new HashSet<IInteract>();
    [ShowOnly] int interactionCount;
    [ShowOnly] IInteract targetInteraction;
    [ShowOnly] IInteract activeInteraction;

    void Update()
    {
        HandleInteractions();

        interactionCount = interactions.Count;
    }

    public void StartInteraction()
    {
        if (targetInteraction != null)
        {
            // Set the target interaction as the active interaction
            activeInteraction = targetInteraction;
            targetInteraction = null;

            // Start the interaction
            activeInteraction.Interact();
        }
        else if (activeInteraction != null)
        {

            // Start the interaction
            activeInteraction.Interact();
        }
    }

    #region ===== [[ INTERACTION HANDLING ]] ===== >>

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable != null)
        {
            Debug.Log("Interactable found: " + other.name);
            interactions.Add(interactable);
        }
        else
        {
            Debug.Log("No interactable component found on: " + other.name);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        IInteract interaction = other.GetComponent<IInteract>();
        if (interaction != null)
        {
            interactions.Remove(interaction);
        }
    }



    void HandleInteractions()
    {
        if (interactions.Count > 0)
        {
            // May want a better priority system, but this is fine for now:
            this.targetInteraction = interactions.First();
            this.targetInteraction.Target();
        }
    }
    #endregion
}
