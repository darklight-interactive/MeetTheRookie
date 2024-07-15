using System.Collections;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Audio
{
    /// <summary>
    ///  This is the main singleton class that manages all FMOD audio events and buses.
    /// </summary>
    [RequireComponent(typeof(StudioEventEmitter))]
    public class FMODEventManager : MonoBehaviourSingleton<FMODEventManager>
    {
        private StudioEventEmitter _studioEventEmitter => GetComponent<StudioEventEmitter>();
        public static EventInstance CurrentSongInstance { get; private set; }
        public static EventDescription CurrentSongDescription { get; private set; }



        #region == [[ BUSES & BANKS ]] ========================================================

        [ShowOnly]
        public FMOD.RESULT busListOk = FMOD.RESULT.ERR_UNIMPLEMENTED;

        [ShowOnly]
        public FMOD.RESULT systemIsOk = FMOD.RESULT.ERR_UNIMPLEMENTED;

        [ShowOnly]
        public bool allBanksLoaded = false;

        [ShowOnly]
        public bool allBusesLoaded = false;

        [SerializeField]
        FMOD.Studio.Bus[] myBuses;

        [Space(10), Header("Buses")]
        [SerializeField]
        private Bus masterBus;

        [SerializeField]
        private string masterBusPath = "bus:/MasterVolume";

        [SerializeField, Range(-80f, 10.0f)]
        private float masterBusVolume;
        #endregion

        public static void PlayOneShot(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        public static PLAYBACK_STATE PlaybackState(EventInstance instance)
        {
            PLAYBACK_STATE pS;
            instance.getPlaybackState(out pS);
            return pS;
        }

        public static void PlayEventWithParameters(
            EventReference eventReference,
            params (string name, float value)[] parameters
        )
        {
            if (eventReference.IsNull)
                return;
            EventInstance instance = RuntimeManager.CreateInstance(eventReference);
            foreach (var (name, value) in parameters)
            {
                instance.setParameterByName(name, value);
            }
            instance.start();
            instance.release();
        }

        public static void PlayEventWithParametersByName(
            EventReference eventReference,
            params (string name, string value)[] parameters
        )
        {
            if (eventReference.IsNull)
                return;
            EventInstance instance = RuntimeManager.CreateInstance(eventReference);
            foreach (var (name, value) in parameters)
            {
                instance.setParameterByNameWithLabel(name, value);
            }
            instance.start();
            instance.release();
        }

        public static string GetInstantiatedEventPath(FMOD.Studio.EventInstance instance)
        {
            string result;
            FMOD.Studio.EventDescription description;

            instance.getDescription(out description);
            description.getPath(out result);

            // expect the result in the form event:/folder/sub-folder/eventName
            return result;
        }

        // --------------------------------- PUBLIC METHODS ---------------------------------
        public override void Initialize()
        {
            LoadBanksAndBuses();
        }

        void Update()
        {
            float volume = Mathf.Pow(10.0f, masterBusVolume / 20f);
            masterBus.setVolume(volume);
        }

        // Coroutine to handle the repeated playing of an event
        private IEnumerator RepeatEventRoutine(EventReference eventReference, float interval)
        {
            while (true)
            {
                FMODEventManager.PlayOneShot(eventReference);
                yield return new WaitForSeconds(interval);
            }
        }


        Coroutine repeatEventCoroutine;
        // Method to start repeating an event
        public void StartRepeatingEvent(EventReference eventReference, float interval)
        {
            repeatEventCoroutine = StartCoroutine(RepeatEventRoutine(eventReference, interval));
        }

        // Method to stop repeating an event
        public void StopRepeatingEvent()
        {
            StopCoroutine(repeatEventCoroutine);
        }



        public void PlaySong(EventReference newSongEventRef)
        {
            if (newSongEventRef.IsNull)
            {
                Debug.LogWarning($"{Prefix} FMOD SONG event path does not exist: " + newSongEventRef);
                return;
            }

            /*
            // If the new song is the same as the current song, do nothing
            string currentEventName = GetInstantiatedEventPath(CurrentSongInstance);
            string newEventName = newSongEventRef.Path;
            if (currentEventName == newEventName)
            {
                Debug.LogWarning($"{Prefix} FMOD SONG event is already playing: " + newSongEventRef);
                return;
            }
            */

            // If the current background music is playing, fade it out and start the new song
            if (CurrentSongInstance.isValid() && PlaybackState(CurrentSongInstance) == PLAYBACK_STATE.PLAYING)
            {
                StartCoroutine(EventTransitionRoutine(newSongEventRef));
                return;
            }

            // Create a new instance of the song and start it
            EventInstance newSongInstance = RuntimeManager.CreateInstance(newSongEventRef);
            CurrentSongInstance = newSongInstance;
            newSongInstance.start();
            newSongInstance.release();
        }

        IEnumerator EventTransitionRoutine(EventReference newSongEventRef)
        {
            if (newSongEventRef.IsNull)
            {
                Debug.LogWarning(
                    $"{Prefix} FMOD SONG event path does not exist: " + newSongEventRef
                );
                yield break;
            }

            // Begin fading out the current song
            CurrentSongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            // Wait for the song to stop playing
            PLAYBACK_STATE playbackState = PlaybackState(CurrentSongInstance);
            while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                CurrentSongInstance.getPlaybackState(out playbackState);
                yield return null;
            }

            // Create a new instance of the song and start it
            EventInstance newSongInstance = RuntimeManager.CreateInstance(newSongEventRef);
            CurrentSongInstance = newSongInstance;
            newSongInstance.start();
            newSongInstance.release();
        }

        public void StopCurrentSong()
        {
            StartCoroutine(StopCurrentSongRoutine());
        }

        public IEnumerator StopCurrentSongRoutine()
        {
            Debug.Log(CurrentSongInstance);
            CurrentSongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PLAYBACK_STATE playbackState = PlaybackState(CurrentSongInstance);
            while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                CurrentSongInstance.getPlaybackState(out playbackState);
                yield return null;
            }

            CurrentSongInstance.release();
        }

        #region == [[ LOAD BANKS AND BUSES ]] ========================================
        public void LoadBanksAndBuses()
        {
            StartCoroutine(LoadBanksAndBusesRoutine());
        }

        public IEnumerator LoadBanksAndBusesRoutine()
        {
            //Console.Log($"{Prefix} Loading.");
            // ============================================= LOAD ============================
            FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
            foreach (FMOD.Studio.Bank bank in loadedBanks)
            {
                // Get the path of the bank
                bank.getPath(out string bankPath);
                // Load the bank
                FMOD.RESULT bankLoadResult = bank.loadSampleData();
                //Console.Log($"{Prefix} Bank Load Result: " + bankPath + " -> " + bankLoadResult);
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
                        /*
                        Console.Log(
                            $"{Prefix} Bus Load Result: " + bankPath + " -> " + bankLoadResult
                        );
                        */
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

            // ------- Confirm Load -------
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

            // If banks and buses are not loaded, try again in 1 second
            if (!allBanksLoaded || !allBusesLoaded)
            {
                yield return new WaitForSeconds(1);
                LoadBanksAndBuses();
            }
        }
        #endregion
    }
}
