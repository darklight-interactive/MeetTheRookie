using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Camera;
using Darklight.UnityExt.Editor;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class MTR_CameraTargeting : MonoBehaviour
{
    public InkyStoryManager storyManager => InkyStoryManager.Instance;
    public CameraController cameraController => GetComponent<CameraController>();
    [ShowOnly, SerializeField] private string _currentSpeaker;


    // Start is called before the first frame update
    void Start()
    {
        // Observer the current speaker variable in the story
        storyManager.OnSpeakerSet += SetSpeakerTarget;
    }

    void SetSpeakerTarget(string speaker)
    {
        _currentSpeaker = speaker;

        // Set the Camera Target to the Player
        if (_currentSpeaker == "Lupe")
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            cameraController.SetFocusTarget(playerController.transform);
            return;
        }

        // Set the Camera Target to a NPC
        NPC_Interactable[] interactables = FindObjectsByType<NPC_Interactable>(FindObjectsSortMode.None);
        foreach (NPC_Interactable interactable in interactables)
        {
            if (interactable.speakerTag.Contains(_currentSpeaker))
            {
                cameraController.SetFocusTarget(interactable.transform);
            }
        }
    }
}
