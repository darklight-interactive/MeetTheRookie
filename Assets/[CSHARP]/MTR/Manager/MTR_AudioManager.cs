using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.FMODExt;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MTR_AudioManager : FMODExt_EventManager
{
    // Overwrite the Instance property to return the instance of this class
    public static new MTR_AudioManager Instance =>
        FMODExt_EventManager.Instance as MTR_AudioManager;

    // Overwrite the generalSFX property to return the instance of the MTR_GeneralSFX class
    public MTR_GeneralSFX generalSFX => base.GeneralSFX as MTR_GeneralSFX;

    // Overwrite the backgroundMusic property to return the instance of the MTR_MusicObject class
    public MTR_MusicObject backgroundMusic => base.BackgroundMusic as MTR_MusicObject;

    public bool HasMasterBankLoaded => FMODUnity.RuntimeManager.HasBankLoaded("Master");

    // Keeps track of looping events
    private Dictionary<string, EventInstance> activeEventInstances =
        new Dictionary<string, EventInstance>();

    #region ///--Accessing FMODExt_EventManager Static Events--///

    public override void Initialize()
    {
        base.Initialize();
        MTRSceneManager.Instance.OnSceneChanged += HandleSceneChange;
    }

    void HandleSceneChange(Scene oldScene, Scene newScene)
    {
        if (HasMasterBankLoaded)
        {
            PlaySceneBackgroundMusic(newScene.name);
        }
        else
        {
            Debug.Log($"{Prefix} >> Waiting for Master Bank to load");
        }
    }

    public override void PlaySceneBackgroundMusic(string sceneName)
    {
        EventReference bg_music = backgroundMusic.GetBackgroundMusicByScene(sceneName);
        backgroundMusic.SetMusicIntensity(sceneName);
        PlaySong(bg_music);
    }

    public void PlayOneShotSFX(EventReference eventReference)
    {
        PlayOneShot(eventReference);
    }

    public void StartRepeatSFX(EventReference eventReference, float interval)
    {
        StartRepeatingEvent(eventReference, interval);
    }

    public void StopRepeatSFX()
    {
        StopRepeatingEvent();
    }
    #endregion

    #region ///--MTR_Specific Functions--///

    GameObject player;
    Coroutine repeatFootstepCoroutine;
    private bool footstepsPlaying; // Boolean to prevent null reference when stopping a non-existent footstep coroutine

    public void StartFootstepEvent()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<MTRPlayerController>().gameObject;
        }

        if (generalSFX != null && !generalSFX.footstep.IsNull)
        {
            repeatFootstepCoroutine = StartCoroutine(
                RepeatEventRoutine(generalSFX.footstep, generalSFX.footstepInterval, player)
            );
        }
        else
        {
            Debug.LogError("Footstep event is null.");
        }
    }

    // Handling spatialized repeated events
    private IEnumerator RepeatEventRoutine(
        EventReference eventReference,
        float interval,
        GameObject soundObject
    )
    {
        footstepsPlaying = true;
        while (true)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(eventReference, soundObject);
            yield return new WaitForSeconds(interval);
        }
    }

    public void StopFootstepEvent()
    {
        if (player == null || !footstepsPlaying)
        {
            return;
        }
        StopCoroutine(repeatFootstepCoroutine);
        footstepsPlaying = false;
    }

    // Inky SFX
    public void PlayOneShotByPath(string eventName)
    {
        string eventPath = "event:/SFX/Inky/" + eventName;
        FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        //Debug.Log($"Started PlaySFX from Inky with SFX: {eventPath}");
    }

    public void StartRepeatSFXByPath(string eventName) //, float interval)
    {
        //Debug.Log("Started PlayLoopingSFX from Inky");
        string eventPath = "event:/SFX/" + eventName;
        EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        activeEventInstances[eventPath] = eventInstance;
        eventInstance.start();
    }

    public void StopRepeatSFXByPath(string eventName)
    {
        //Debug.Log("Started StopLoopingSFX");
        string eventPath = "event:/SFX/" + eventName;
        if (activeEventInstances.ContainsKey(eventPath))
        {
            activeEventInstances[eventPath].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            activeEventInstances[eventPath].release();
            activeEventInstances.Remove(eventPath);
            return;
        }
        Debug.LogWarning($"Could not stop SFX because it wasn't instantiated: {eventPath}");
    }
    #endregion

    #region ///--UNUSED--///
    //public void SetReverb(float reverbValue)
    //{
    //    if (reverbValue >= 0 &&  reverbValue <= 1) { FMODUnity.RuntimeManager.StudioSystem.setParameterByName(Reverb, reverbValue); }
    //}

    //public void SetFMODGlobalParameter(string parameterName, float parameterValue)
    //{
    //    FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    //    Debug.Log("Set parameter " + parameterName + " to " + parameterValue);
    //}

    //public string GetEventPath(EventReference eventReference)
    //{
    //    FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
    //    string eventPath = GetInstantiatedEventPath(eventInstance);
    //    eventInstance.release();
    //    return eventPath;
    //}

    //private IEnumerator RepeatEventRoutine(string eventPath)
    //{
    //    isInkySFXLooping = true;
    //    while (true)
    //    {
    //        FMODUnity.RuntimeManager.PlayOneShot(eventPath);
    //        yield return new WaitForSeconds(1.0f); // Originally used "interval" for controlled nuance
    //    }
    //}
    #endregion
}
