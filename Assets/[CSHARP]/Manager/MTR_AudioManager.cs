using Darklight.UnityExt.FMODExt;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class MTR_AudioManager : FMODExt_EventManager
{
    // Overwrite the Instance property to return the instance of this class
    public static new MTR_AudioManager Instance => FMODExt_EventManager.Instance as MTR_AudioManager;

    // Overwrite the generalSFX property to return the instance of the MTR_GeneralSFX class
    public new MTR_GeneralSFX generalSFX => base.GeneralSFX as MTR_GeneralSFX;
    // Overwrite the backgroundMusic property to return the instance of the MTR_MusicObject class
    public new MTR_MusicObject backgroundMusic => base.BackgroundMusic as MTR_MusicObject;

    public new void PlaySceneBackgroundMusic(string sceneName)
    {
        EventReference bg_music = backgroundMusic.GetBackgroundMusicByScene(sceneName);
        backgroundMusic.SetMusicIntensity(sceneName);
        PlaySong(bg_music);
    }

    //public void SetReverb(float reverbValue)
    //{
    //    if (reverbValue >= 0 &&  reverbValue <= 1) { FMODUnity.RuntimeManager.StudioSystem.setParameterByName(Reverb, reverbValue); }
    //}

    //public void SetFMODGlobalParameter(string parameterName, float parameterValue)
    //{
    //    FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    //    Debug.Log("Set parameter " + parameterName + " to " + parameterValue);
    //}


    GameObject player;
    Coroutine repeatFootstepCoroutine;
    public void StartFootstepEvent()
    {
        if (player == null) { player = GameObject.Find("[PLAYER] Lupe"); }
        
        repeatFootstepCoroutine = StartCoroutine(RepeatEventRoutine(generalSFX.footstep, generalSFX.footstepInterval, player));
    }

    // Handling spatialized repeated events
    private IEnumerator RepeatEventRoutine(EventReference eventReference, float interval, GameObject soundObject)
    {
        while (true)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(eventReference, soundObject);
            yield return new WaitForSeconds(interval);
        }
    }

    public void StopFootstepEvent()
    {
        StopCoroutine(repeatFootstepCoroutine);
    }
}
