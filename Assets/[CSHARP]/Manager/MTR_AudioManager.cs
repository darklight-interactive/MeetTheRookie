using Darklight.UnityExt.Audio;
using FMODUnity;
using UnityEngine;

public class MTR_AudioManager : FMODEventManager
{
    public static new MTR_AudioManager Instance => FMODEventManager.Instance as MTR_AudioManager;

    public EventReference footstepEventReference;

    public void PlayFootstepEvent()
    {
        StartRepeatingEvent(footstepEventReference, 0.25f);
    }

    public EventReference firstInteractionEventReference;
    public void PlayFirstInteractionEvent()
    {
        PlayOneShot(firstInteractionEventReference);
    }
}
