using UnityEngine;

public class NPCInteraction : Interaction, IInteraction
{
    public override void Interact()
    {
        base.Interact();
        Debug.Log("NPC Interaction");

        UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(promptUITarget.position);
    }
}