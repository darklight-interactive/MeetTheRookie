using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(MTRCameraRig))]
public class MTRCameraController : MonoBehaviour, IUnityEditorListener
{
    MTRCameraRig _rig;

    InkyStoryManager StoryManager => InkyStoryManager.Instance;
    List<string> _speakerList => InkyStoryManager.SpeakerList;
    public MTRCameraRig Rig
    {
        get
        {
            if (_rig == null)
            {
                _rig = GetComponent<MTRCameraRig>();
            }
            return _rig;
        }
    }


    [Dropdown("_speakerList"), SerializeField, ShowOnly] public string currentSpeaker;

    public void OnEditorReloaded()
    {
        MTRSceneManager.Instance.CameraBoundsLibrary.GetActiveCameraBounds(out MTRCameraRigBounds cameraBounds);
        if (cameraBounds != null && Rig.Bounds != cameraBounds)
        {
            Rig.SetBounds(cameraBounds);
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        // Observe the current speaker variable in the story
        StoryManager.OnSpeakerSet += SetSpeakerTarget;
    }

    void OnDestroy()
    {
        StoryManager.OnSpeakerSet -= SetSpeakerTarget;
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
