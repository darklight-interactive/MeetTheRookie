using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Darklight.Game.Utility;
using static Darklight.UnityExt.CustomInspectorGUI;

/// <summary>
///  This is the main singleton class that manages all FMOD audio events and buses.
/// </summary>
public class FMODManager : MonoBehaviourSingleton<FMODManager>
{
    [ShowOnly] public FMOD.RESULT busListOk;
    [ShowOnly] public FMOD.RESULT sysemIsOk;
    [ShowOnly] public bool allBanksLoaded = false;
    [ShowOnly] public bool allBusesLoaded = false;

    [SerializeField] FMOD.Studio.Bus[] myBuses;

    [Space(10), Header("Buses")]
    [SerializeField] private Bus masterBus;
    [SerializeField] private string masterBusPath = "bus:/MasterVolume";
    [SerializeField, Range(-80f, 10.0f)] private float masterBusVolume;


    [Space(10), Header("Background Music Event")]
    [SerializeField] private StudioEventEmitter backgroundEmitter;
    public EventReference backgroundMusicEvent;
    public EventInstance backgroundMusicInstance;

    [Space(10), Header("Interaction")]
    [SerializeField] public EventReference interactionEvent;

    public EventInstance currentPlaying;
    protected FMOD.Studio.PLAYBACK_STATE playbackState;


    private void Start()
    {
        backgroundEmitter = GetComponent<StudioEventEmitter>();

        backgroundMusicInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();

        LoadBanksAndBuses();

        Console.Log($"{Prefix} Initialized.");
    }

    void Update()
    {
        float volume = Mathf.Pow(10.0f, masterBusVolume / 20f);
        masterBus.setVolume(volume);
    }




    #region == [[ PLAY EVENTS ]] ==================================================

    public void PlayEvent(EventReference eventReference)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        instance.start();
        instance.release();
    }
    #endregion

    #region == old code ===========================================================
    /*
    //plays a one shot given the fmod event path
    public void Play(string path, Dictionary<string, float> parameters = null)
    {
        EventInstance instance = RuntimeManager.CreateInstance(path);
        if (parameters != null)
        {
            foreach (KeyValuePair<string, float> val in parameters)
            {
                instance.setParameterByName(val.Key, val.Value);
            }
        }
        instance.start();
        instance.release();
        Debug.Log("[Audio Manager] playing one shot: " + path);
    }

    public EventInstance Play(string path)
    {

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return RuntimeManager.CreateInstance(path); //NEEDS TO BE CHANGED
        }

        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();
        instance.release();
        //Debug.Log("[Audio Manager] playing one shot: " + path);
        return instance;
    }



    public void PlaySong(string path)
    {
        Debug.Log("[Audio Manager] Playing Song: " + path);
        if (currentPlaying.isValid())
        {
            StartCoroutine(RestOfPlaySong(path));
        }
        else
        {
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
                return;
            }

            EventInstance song = RuntimeManager.CreateInstance(path);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    public IEnumerator RestOfPlaySong(string path)
    {
        Debug.Log(currentPlaying);
        currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentPlaying.getPlaybackState(out playbackState);
        while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            currentPlaying.getPlaybackState(out playbackState);
            yield return null;
        }

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
        }
        else
        {
            yield return new WaitForSeconds(1);
            EventInstance song = RuntimeManager.CreateInstance(path);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    public void StopCurrentSong()
    {
        StartCoroutine(StopCurrentSongRoutine());
    }

    public IEnumerator StopCurrentSongRoutine()
    {
        Debug.Log(currentPlaying);
        currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentPlaying.getPlaybackState(out playbackState);
        while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            currentPlaying.getPlaybackState(out playbackState);
            yield return null;
        }
    }
*/
    #endregion




    #region == [[ LOAD BANKS AND BUSES ]] ========================================

    public void LoadBanksAndBuses()
    {
        StartCoroutine(LoadBanksAndBusesRoutine());
    }

    public IEnumerator LoadBanksAndBusesRoutine()
    {
        Console.Log($"{Prefix} Loading.");

        // ============================================= LOAD ============================
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
        foreach (FMOD.Studio.Bank bank in loadedBanks)
        {
            // Get the path of the bank
            bank.getPath(out string bankPath);

            // Load the bank
            FMOD.RESULT bankLoadResult = bank.loadSampleData();
            Console.Log($"{Prefix} Bank Load Result: " + bankPath + " -> " + bankLoadResult);


            // Retrieve the list of buses associated with the bank
            busListOk = bank.getBusList(out myBuses);

            // Get the number of buses in the bank
            bank.getBusCount(out int busCount);

            if (busCount > 0)
            {
                // Iterate through the buses in the bank
                foreach (Bus bus in myBuses)
                {
                    // Get the path of each bus
                    bus.getPath(out string busPath);

                    // Load the bus
                    FMOD.RESULT busLoadResult = bus.lockChannelGroup();
                    Console.Log($"{Prefix} Bus Load Result: " + bankPath + " -> " + bankLoadResult);

                    // Save the bus to the appropriate variable
                    if (busPath == masterBusPath)
                    {
                        masterBus = FMODUnity.RuntimeManager.GetBus(busPath);
                        // masterBus.setVolume(masterBusVolume);

                    }
                    /*
                    else if (busPath == musVolBusPath)
                    {
                        musBus = bus;
                        musBus.setVolume(musicVolume);

                    }
                    else if (busPath == sfxVolBusPath)
                    {
                        sfxBus = bus;
                        sfxBus.setVolume(sfxVolume);

                    }
                    else if (busPath == diaVolBusPath)
                    {
                        diaBus = bus;
                        diaBus.setVolume(dialogueVolume);

                    }
                    else if (busPath == ambiVolBusPath)
                    {
                        ambiBus = bus;
                        ambiBus.setVolume(ambianceVolume);
                    }
                    */
                }
            }
        }

        // ========================================== CONFIRM LOAD =======================

        foreach (FMOD.Studio.Bank bank in loadedBanks)
        {
            // Load each bank
            bank.loadSampleData();

            // Check if all banks are loaded
            FMOD.Studio.LOADING_STATE bankLoadingState;
            bank.getLoadingState(out bankLoadingState);

            if (!allBanksLoaded && bankLoadingState == FMOD.Studio.LOADING_STATE.LOADED)
            {
                allBanksLoaded = true;
            }

            // Retrieve the list of buses associated with the bank
            bank.getBusList(out FMOD.Studio.Bus[] buses);

            foreach (FMOD.Studio.Bus bus in buses)
            {
                /*
                // Check if the bus is already locked
                if (!bus.isLocked)
                {
                    // Lock the bus
                    bus.lockChannelGroup();
                }
                */

                // Check if all buses are loaded
                if (!allBusesLoaded && bus.isValid())
                {
                    allBusesLoaded = true;
                }
            }
        }


        Debug.Log("AudioManager : All Banks Loaded " + allBanksLoaded);
        Debug.Log("AudioManager : All Buses Loaded " + allBusesLoaded);

        if (!allBanksLoaded || !allBusesLoaded)
        {
            yield return new WaitForSeconds(1);
            LoadBanksAndBuses();
        }
    }
    #endregion

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(FMODManager))]
public class FMODManagerEditor : UnityEditor.Editor
{
    private void OnEnable()
    {
        FMODManager FMODManager = (FMODManager)target;
    }

    public override void OnInspectorGUI()
    {
        FMODManager FMODManager = (FMODManager)target;

        FMODManager.Console.DrawInEditor();

        DrawDefaultInspector();
    }
}
#endif