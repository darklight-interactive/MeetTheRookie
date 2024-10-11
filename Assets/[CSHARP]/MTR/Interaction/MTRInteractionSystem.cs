using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTRInteractionSystem : InteractionSystem
{
    public static MTRPlayerInteractor PlayerInteractor
    {
        get
        {
            Registry.TryGetInteractable(out MTRPlayerInteractor playerInteractor);
            return playerInteractor;
        }
    }
    public static MTRPlayerInput PlayerInputController
    {
        get
        {
            return PlayerInteractor.GetComponent<MTRPlayerInput>();
        }
    }

    public static void TryGetInteractableByStitch(string stitchName, out MTRInteractable interactable)
    {
        interactable = null;

        foreach (MTRInteractable i in Registry.Interactables.Values)
        {
            if (i.InteractionStitch == stitchName)
            {
                interactable = i;
                return;
            }
        }
    }
}
