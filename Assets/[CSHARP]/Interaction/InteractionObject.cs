using System;
using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.Grid2D;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionObject : OverlapGrid2D, IInteract
{
    [SerializeField] private string _ink_knot;
    public string ink_knot { get => _ink_knot; set => _ink_knot = value; }
    public Darklight.Console console => new Darklight.Console();
    public int counter { get; set; }

    public void Target()
    {
        //Vector3? worldPostion = GetBestWorldPosition();

        Vector3? worldPostion = null;
        if (worldPostion == null) worldPostion = transform.position;
        UXML_InteractionUI.Instance.DisplayInteractPrompt((Vector3)worldPostion);
    }

    /*
        public Vector3? GetBestWorldPosition()
        {
            Vector2Int? bestPosition = GetBestPositionKey();
            Debug.Log($"GetBestWorldPosition >> {bestPosition}");
            if (bestPosition == null) return null;

            IGrid2DData data = dataGrid.GetData((Vector2Int)bestPosition);
            return data.worldPosition;
        }
        */

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
