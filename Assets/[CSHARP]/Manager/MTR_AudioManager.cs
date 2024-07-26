using Darklight.UnityExt.Audio;
using FMODUnity;
using UnityEngine;

public class MTR_AudioManager : FMOD_EventManager
{
    // Overwrite the Instance property to return the instance of this class
    public static new MTR_AudioManager Instance => FMOD_EventManager.Instance as MTR_AudioManager;

    // Overwrite the generalSFX property to return the instance of the MTR_GeneralSFX class
    private new MTR_GeneralSFX generalSFX => base.generalSFX as MTR_GeneralSFX;

    public void StartFootstepEvent()
    {
        StartRepeatingEvent(generalSFX.footstep, generalSFX.footstepInterval);
    }
}
