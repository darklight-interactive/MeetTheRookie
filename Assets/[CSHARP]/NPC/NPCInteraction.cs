using UnityEngine;

public class NPCInteraction : Interactable
{
    public override void Interact()
    {
        base.Interact();
        Debug.Log("NPC Interaction");
    }
}