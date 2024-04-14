using System;
using System.Collections;
using System.Collections.Generic;
using Darklight.Console;
using Darklight.Game.Grid;
using UnityEngine;

public interface IInteraction
{
    public string ink_knot { get; }
    public int counter { get; set; }
    public abstract void Target();
    public virtual void Interact()
    {
        counter++;
    }
    public virtual void Reset()
    {
        counter = 0;
    }
}

[RequireComponent(typeof(BoxCollider2D))]
public class Interaction : Grid2D_OverlapGrid, IInteraction
{
    public string ink_knot { get; private set; } = "default";
    public int counter { get; set; }
    public virtual void Target()
    {
        Vector3? worldPostion = null;
        if (worldPostion == null) worldPostion = transform.position;
        UXML_InteractionUI.Instance.DisplayInteractPrompt((Vector3)worldPostion);
    }

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
        UXML_InteractionUI.Instance.HideInteractPrompt();
    }

}
