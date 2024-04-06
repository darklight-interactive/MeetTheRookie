using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StoryManager;

[RequireComponent(typeof(Collider2D))]
public class InkInteraction : MonoBehaviour
{
    public string inkKnot = "BLANK";

    PlayerController c;

    private void OnTriggerEnter2D(Collider2D other) {
        var controller = other.GetComponentInParent<PlayerController>();
        if (controller != null) {
            controller.SubscribeInteraction(this);
            c = controller;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (c != null) {
            c.UnsubscribeInteraction(this);
            c = null;
        }
    }

    /// <summary>
    /// Start a general interaction.
    /// </summary>
    /// <param name="onComplete">A callback to call when we're done with the present interaction.</param>
    public void Interact(KnotComplete onComplete) {
        StoryManager.Instance.Run(inkKnot, transform, onComplete);
    }
    /// <summary>
    /// From <see cref="PlayerController"/>, for when the player is pressing Z while we haven't relenquished control.
    /// </summary>
    public void Interact() {
        StoryManager.Instance.Continue();
    }

    /// <summary>
    /// If the player is pressing the move inputs during our interaction.
    /// </summary>
    /// <param name="move">The player's desired move.</param>
    public void MoveInteract(Vector2 move) {
        StoryManager.Instance.MoveUpdate(move);
    }
}
