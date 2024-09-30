using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(MTRCameraRig))]
public class MTRCameraController : MonoBehaviour
{
    InkyStoryManager StoryManager => InkyStoryManager.Instance;
    List<string> _speakerList => InkyStoryManager.SpeakerList;
    public MTRCameraRig Rig => GetComponent<MTRCameraRig>();

    [Dropdown("_speakerList"), SerializeField, ShowOnly] public string currentSpeaker;

    // Start is called before the first frame update
    void Start()
    {
        // Observe the current speaker variable in the story
        StoryManager.OnSpeakerSet += SetSpeakerTarget;
    }

    [Button]
    void SetSpeakerAsFollowTarget()
    {
        SetSpeakerTarget(currentSpeaker);
    }

    void SetSpeakerTarget(string speaker)
    {
        currentSpeaker = speaker;

        // Set the Camera Target to the Player
        if (currentSpeaker == "Lupe")
        {
            MTRPlayerInteractor player = FindObjectsByType<MTRPlayerInteractor>(FindObjectsSortMode.None)[0];
            Rig.SetFollowTarget(player.transform);
            return;
        }

        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(FindObjectsSortMode.None);
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag.Contains(currentSpeaker))
            {
                Rig.SetFollowTarget(interactable.transform);
            }
        }
    }

    public void SetPlayerAsFollowTarget()
    {
        MTRPlayerInteractor player = MTRInteractionSystem.PlayerInteractor;
        Rig.SetFollowTarget(player.transform);
    }
}
