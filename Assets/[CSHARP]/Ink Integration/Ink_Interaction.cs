using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ink_StoryManager;

[RequireComponent(typeof(Collider2D))]
public class Ink_Interaction : MonoBehaviour
{
    Ink_StoryManager inkStoryManager => Ink_StoryManager.Instance;

    public string inkKnot = "BLANK";

    public void StartInteractionKnot(KnotComplete onComplete)
    {
        inkStoryManager.Run(inkKnot, this, onComplete);

    }

    /// <summary>
    /// If the player is pressing the move inputs during our interaction.
    /// </summary>
    /// <param name="move">The player's desired move.</param>
    public void MoveInteract(Vector2 move)
    {
        Ink_StoryManager.Instance.MoveUpdate(move);
    }
}
