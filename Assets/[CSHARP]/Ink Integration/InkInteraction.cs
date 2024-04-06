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

    public void Interact(KnotComplete onComplete) {
        StoryManager.Instance.Run(inkKnot, transform, onComplete);
    }
    public void Interact() {
        StoryManager.Instance.Continue();
    }
}
