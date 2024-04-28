using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

//EVENTS W/ PARAMETERS
//music events, isPaused
//the truck engine, 
//wind ambiance, 
//customer The code you provided is a comment in the code. Comments are used to provide explanations or notes within the code for better understanding by developers. In this case, the comment is indicating that the code below it is related to an order.
public class FMOD_AudioManager : MonoBehaviour
{
    GameManager gameManager;
    public string bankDirectoryPath = ""; // Directory path to the banks


    //SLIDERS FOR VOLUME, SHOULD BE A VALUE BETWEEN 0 & 1
    [Header("Volumes (sliders)")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;
    [Range(0f, 1f)]
    public float dialogueVolume = 0.5f;
    [Range(0f, 1f)]
    public float ambianceVolume = 0.5f;

    [Header("Bus Paths")]
    public string masVolBusPath = "bus:/";
    public string musVolBusPath = "bus:/Music";
    public string sfxVolBusPath = "bus:/SFX";
    public string diaVolBusPath = "bus:/Dialogue";
    public string ambiVolBusPath = "bus:/Ambience";

    //BUSES

    public FMOD.Studio.Bus masBus;
    public FMOD.Studio.Bus musBus;
    public FMOD.Studio.Bus sfxBus;
    public FMOD.Studio.Bus diaBus;
    public FMOD.Studio.Bus ambiBus;

    public bool isPaused;

    #region /////////////////////////MUSIC//////////////////////////////

    [Header("FMOD Music")]

    [Tooltip("FMOD Event Path for the folder that contains all the music")]
    public string menuMusicPath = "event:/Music/MenuMusic";
    public string storyMusicPath = "event:/Music/StoryMusic";
    public string tacoMusicPath = "event:/Music/TacoMusic";
    public string drivingMusicPath = "event:/Music/DrivingMusic";

    public string endingAmbiencePath = "event:/Music/Credits/HappyEnding&Ambience";
    public string sadCreditsPath = "event:/Music/Credits/SadEnding";
    #endregion

    #region /////////////////////////AMBIENCE//////////////////////////////
    [Header("FMOD Driving(Ambience) Event Path Strings")]

    [Tooltip("path of ambience event")]
    public string drivingAmbiPath;
    #endregion

    #region /////////////////////////SFX//////////////////////////////

    // CUTSCENE
    [Header("FMOD Cutscene(SFX) Event Path Strings")]

    [Tooltip("path of receieve text event")]
    public string recieveTextSFX;
    [Tooltip("path of the sending text event")]
    public string sendTextSFX;
    [Tooltip("path of the typing event")]
    public string typingSFX;
    public string skipSFX;



    // DRIVING

    [Header("FMOD Driving(SFX) Event Path Strings")]

    [Tooltip("FMOD Event Path for the folder that contains all the Driving SFX")]
    public string drivingSFXPath;

    [Tooltip("Name of Nitro Boost event")]
    public string nitroBoostSFX; //IMPLEMENTED
    [Tooltip("Name of the Successful Flip event")]
    public string flipBoostSFX; //IMPLEMENTED

    public string truckLandingSFX; //IMPLEMENTED
    public string crashSFX;

    public string rpmSFX;


    //TACO MAKING

    [Header("FMOD Taco(SFX) Event Path Strings")]

    [Tooltip("Name of taco submission event")]
    public string submitTacoSFX;
    [Tooltip("Name of the ingredient placement event")]
    public string ingriPlaceSFX; //IMPLEMENTED
    [Tooltip("Name of the paw swiping event")]
    public string pawSwipeSFX; //IMPLEMENTED
    public string bellDingSFX;
    public string orderDia;

    #endregion

    #region /////////////////////////UI//////////////////////////////
    [Header("FMOD UI(SFX) Event Path Strings")]

    [Tooltip("path of UI Select")]

    public string selectUI = "event:/SFX/UI & Menu/UI Select";
    public string hoverUI = "event:/SFX/UI & Menu/UI Hover";
    public string creakHoverUI = "event:/SFX/UI & Menu/UI Hover Sign";
    public string sliderUI = "event:/SFX/UI & Menu/UI Slider Feedback";
    public string signDrop = "event:/SFX/UI & Menu/Sign Drop";
    public string meow = "event:/SFX/UI & Menu/Meow Button";
    public string beep = "event:/SFX/UI & Menu/Beep-Beep Button";


    #endregion
    //need to have name of parameter and variable
    //FOR GLOBAL PARAMETERS FMOD Parameter name, variable name
    //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("", x);

    private FMOD.Studio.Bus[] myBuses = new FMOD.Studio.Bus[12];
    private string busesList;
    private string buf;
    private FMOD.Studio.Bank myBank;

    private string BusPath;
    public FMOD.RESULT busListOk;
    public FMOD.RESULT sysemIsOk;

    public EventInstance currentPlaying;
    protected EventInstance currentAmbience;

    public EventInstance currentRPM;

    protected FMOD.Studio.PLAYBACK_STATE playbackState;

    public bool allBanksLoaded = false;
    public bool allBusesLoaded = false;

    void Awake()
    {
        //gameManager = GameManager.instance;

        /*
        // Load the FMOD banks from the specified directory
        RuntimeManager.LoadBank(bankDirectoryPath + "/Master");
        RuntimeManager.LoadBank(bankDirectoryPath + "/SFX");
        RuntimeManager.LoadBank(bankDirectoryPath + "/Music");
        RuntimeManager.LoadBank(bankDirectoryPath + "/Dialogue");
        RuntimeManager.LoadBank(bankDirectoryPath + "/Ambience");

        masBus = FMODUnity.RuntimeManager.GetBus(masVolBusPath);
        musBus = FMODUnity.RuntimeManager.GetBus(musVolBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxVolBusPath);
        diaBus = FMODUnity.RuntimeManager.GetBus(diaVolBusPath);
        ambiBus = FMODUnity.RuntimeManager.GetBus(ambiVolBusPath);
        */

        //menuMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + menuMusic);
        //cutsceneMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + cutsceneMusic);
        //tacoMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + tacoMusic);
        //drivingMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + drivingMusic);*/
    }


    private void Start()
    {
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
            Debug.Log("|| AUDIOMANAGER || Bank Load Result: " + bankPath + " -> " + bankLoadResult);


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
                    Debug.Log("|| AUDIOMANAGER || Bus Load Result: " + busPath + " -> " + busLoadResult);

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

    public void PlayDrivingAmbience(float value)
    {
        if (currentAmbience.isValid())
        {
            currentAmbience.setParameterByName("carHeight", value);
            Debug.Log("[Audio Manager] Driving Ambience updated: " + value);
        }
        else
        {
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(drivingAmbiPath, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("FMOD SONG event path does not exist: " + drivingAmbiPath);
                return;
            }

            EventInstance ambience = RuntimeManager.CreateInstance(drivingAmbiPath);
            currentAmbience = ambience;
            ambience.start();
            ambience.release();
            Debug.Log("[Audio Manager] New Driving Ambience Event: " + value);
        }

    }
    public void StopDrivingAmbience()
    {
        Debug.Log("[Audio Manager] Stopping Driving Ambience");
        if (currentAmbience.isValid())
        {
            currentAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlayRPM(float value)
    {
        if (currentRPM.isValid())
        {
            currentRPM.setParameterByName("RPM", value);
            //Debug.Log("[Audio Manager] RPM updated: " + value);
        }
        else
        {
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(rpmSFX, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("FMOD SONG event path does not exist: " + rpmSFX);
                return;
            }

            EventInstance rpm = RuntimeManager.CreateInstance(rpmSFX);
            currentRPM = rpm;
            rpm.start();
            rpm.release();
            Debug.Log("[Audio Manager] New RPM Event: " + value);
        }
    }
    public void StopRPM()
    {
        Debug.Log("[Audio Manager] Stopping RPM");
        if (currentRPM.isValid())
        {
            currentRPM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void playSelect()
    {
        Play(selectUI);
    }
    public void playHover()
    {
        Play(hoverUI);
    }
    public void playCreakHover()
    {
        Debug.Log(creakHoverUI);
        Play(creakHoverUI);
    }
    public void playSlider()
    {
        Play(sliderUI);
    }
    public void playDrop()
    {
        Play(signDrop);
    }
    public void playMeow()
    {
        Play(meow);
    }
    public void playBeep()
    {
        Play(beep);
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isPaused", isPaused ? 0 : 1);
        masBus.setVolume(masterVolume);
        musBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
        diaBus.setVolume(dialogueVolume);
        ambiBus.setVolume(ambianceVolume);
    }
}