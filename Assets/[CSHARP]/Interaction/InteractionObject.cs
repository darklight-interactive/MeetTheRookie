using System;
using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;



public class InteractionObject : MonoBehaviour, IInteract
{
    public string ink_knot { get; set; }
    public Vector3 world_position => transform.position;
    public Darklight.Console console => new Darklight.Console();
    public int counter { get; set; }
    public virtual void Interact()
    {
        counter++;
        Debug.Log($"Interact >> {counter}");
    }

    public virtual void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        InkyKnotThreader.Instance.GoToKnotAt(ink_knot);
        InkyKnotThreader.Instance.ContinueStory();
    }

    public virtual void ResetInteraction()
    {
        //interactionUI.HideInteractPrompt();
    }
}
