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
    [SerializeField] private string masVolBusPath = "bus:/MasterVolume";


    [Space(10), Header("Volume Control")]
    [SerializeField]
    [Range(-80f, 10f)]
    private float busVolume;
    private float volume;





    [Space(10), Header("Background Music Event")]
    [SerializeField] private StudioEventEmitter backgroundEmitter;
    public EventReference backgroundMusicEvent;
    public EventInstance backgroundMusicInstance;


    public EventInstance currentPlaying;
    protected FMOD.Studio.PLAYBACK_STATE playbackState;


    private void Start()
    {
        backgroundEmitter = GetComponent<StudioEventEmitter>();

        backgroundMusicInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();

        LoadBanksAndBuses();
    }

    public void LoadBanksAndBuses()
    {
        StartCoroutine(LoadBanksAndBusesRoutine());
    }

    public IEnumerator LoadBanksAndBusesRoutine()
    {
        Debug.Log("  << LOAD BANKS AND BUSES >>  ");

        // ============================================= LOAD ============================
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
        foreach (FMOD.Studio.Bank bank in loadedBanks)
        {
            // Get the path of the bank
            bank.getPath(out string bankPath);

            // Load the bank
            FMOD.RESULT bankLoadResult = bank.loadSampleData();
            Debug.Log($"{Prefix} Bank Load Result: " + bankPath + " -> " + bankLoadResult);


            // Retrieve the list of buses associated with the bank
            busListOk = bank.getBusList(out myBuses);

            int busCount;
            string busPath;
            bank.getBusCount(out busCount);
            if (busCount > 0)
            {
                // Iterate through the buses in the bank
                foreach (var bus in myBuses)
                {
                    // Get the path of each bus
                    bus.getPath(out busPath);

                    // Load the bus
                    FMOD.RESULT busLoadResult = bus.lockChannelGroup();
                    Debug.Log($"{Prefix} Bus Load Result: " + bankPath + " -> " + bankLoadResult);

                    /*
                    // Save the bus to the appropriate variable
                    if (busPath == masVolBusPath)
                    {
                        masBus = FMODUnity.RuntimeManager.GetBus(busPath);
                        masBus.setVolume(masterVolume);

                    }
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

    //plays a one shot given the fmod event path
    public void Play(string path, Dictionary<string, float> parameters = null)
    {
        var instance = RuntimeManager.CreateInstance(path);
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

        var instance = RuntimeManager.CreateInstance(path);
        instance.start();
        instance.release();
        //Debug.Log("[Audio Manager] playing one shot: " + path);
        return instance;
    }

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]


    /////////////////////////VOLUME//////////////////////////////
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
}