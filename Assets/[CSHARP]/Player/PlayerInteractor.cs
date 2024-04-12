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
    public PlayerUIHandler playerUIHandler => GetComponent<PlayerUIHandler>();
    protected HashSet<InkyInteraction> interactions = new HashSet<InkyInteraction>();
    [ShowOnly] InkyInteraction targetInteraction;
    [ShowOnly] InkyInteraction activeInteraction;

    void FixedUpdate()
    {
        HandleInteractions();
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
        InkyInteraction interaction = other.GetComponent<InkyInteraction>();
        if (interaction != null)
        {
            interactions.Add(interaction);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        InkyInteraction interaction = other.GetComponent<InkyInteraction>();
        if (interaction != null)
        {
            interactions.Remove(interaction);
        }
    }

    void HandleInteractions()
    {
        if (interactions.Count == 0)
        {
            UXML_InteractionUI.Instance.HideInteractPrompt();
            return;
        }
        else
        {
            // May want a better priority system, but this is fine for now:
            this.targetInteraction = interactions.First();
            UXML_InteractionUI.Instance.DisplayInteractPrompt(targetInteraction.transform.position);
        }
    }
    #endregion
}
