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
            if (playerInteractor == null)
                playerInteractor = FindFirstObjectByType<MTRPlayerInteractor>();
            return playerInteractor;
        }
    }
    public static MTRPlayerInput PlayerInputController
    {
        get { return PlayerInteractor.GetComponent<MTRPlayerInput>(); }
    }

    public static void TryGetInteractableByStitch(
        string stitchName,
        out MTRInteractable interactable
    )
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

    public static void GetSpawnPointInteractables(out List<MTRInteractable> interactables)
    {
        // Get interactables that are spawn points
        interactables = new List<MTRInteractable>();
        foreach (MTRInteractable i in Registry.Interactables.Values)
        {
            if (i.Data.IsSpawnPoint)
                interactables.Add(i);
        }

        // Sort the interactables by spawn index
        interactables.Sort((a, b) => a.Data.SpawnIndex.CompareTo(b.Data.SpawnIndex));
    }

    public static void ResetAllInteractablesExcept(List<MTRInteractable> whitelist)
    {
        foreach (MTRInteractable interactable in Registry.Interactables.Values)
        {
            if (!whitelist.Contains(interactable))
            {
                interactable.Reset();
            }
        }
    }
}
