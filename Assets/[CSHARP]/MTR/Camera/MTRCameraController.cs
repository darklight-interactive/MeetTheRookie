using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

[RequireComponent(typeof(MTRCameraRig))]
public class MTRCameraController : MonoBehaviour
{
    InkyStoryManager StoryManager => InkyStoryManager.Instance;
    public MTRCameraRig Rig => GetComponent<MTRCameraRig>();

    [ShowOnly, SerializeField] private string _currentSpeaker;

    public bool targetBasedOnSpeaker = false;

    // Start is called before the first frame update
    void Start()
    {
        // Observer the current speaker variable in the story
        if (!targetBasedOnSpeaker) { StoryManager.OnSpeakerSet += SetSpeakerTarget; }
    }

    void SetSpeakerTarget(string speaker)
    {
        _currentSpeaker = speaker;

        // Set the Camera Target to the Player
        if (_currentSpeaker == "Lupe")
        {
            MTRPlayerInteractor player = FindObjectsByType<MTRPlayerInteractor>(FindObjectsSortMode.None)[0];
            Rig.SetFollowTarget(player.transform);
            return;
        }

        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(FindObjectsSortMode.None);
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag.Contains(_currentSpeaker))
            {
                Rig.SetFollowTarget(interactable.transform);
            }
        }
    }
}
