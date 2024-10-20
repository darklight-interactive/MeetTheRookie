using System.Collections;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;

using FMOD;
using FMOD.Studio;

using FMODUnity;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NaughtyAttributes;

namespace Darklight.UnityExt.FMODExt
{
    /// <summary>
    ///  This is the main singleton class that manages all FMOD audio events and buses.
    /// </summary>
    [RequireComponent(typeof(StudioEventEmitter))]
    public class FMODExt_EventManager : MonoBehaviourSingleton<FMODExt_EventManager>
    {
        public static EventInstance CurrentSongInstance { get; private set; }
        public static EventDescription CurrentSongDescription { get; private set; }
        public static ConsoleGUI InternalConsole { get; private set; } = new ConsoleGUI();

        #region == [[ MONOBEHAVIOUR SINGLETON METHODS ]] ==================================== >>
        public override void Initialize()
        {
            LoadBanksAndBuses();
        }

        public void Update()
        {
            // << UPDATE BUS DATA >>
            foreach (FMODExt_Bus bus in _busData)
            {
                bus.Update();
            }
        }
        #endregion

        #region == [[ FMODExt EVENT OBJECTS ]] ============================================== >>

        [Header("FMOD Event Objects")]
        [SerializeField, Expandable] FMODExt_MusicObject _backgroundMusic;
        [SerializeField, Expandable] FMODExt_SFXObject _generalSFX;
        public FMODExt_MusicObject BackgroundMusic => _backgroundMusic;
        public FMODExt_SFXObject GeneralSFX => _generalSFX;

        #region -- (( PLAY EVENTS )) ----------------------- ))
        public virtual void PlaySceneBackgroundMusic(string sceneName)
        {
            EventReference bg_music = _backgroundMusic.GetBackgroundMusicByScene(sceneName);
            PlaySong(bg_music);
        }

        public void PlayStartInteractionEvent()
        {
            PlayOneShot(_generalSFX.startInteraction);
        }

        public void PlayContinuedInteractionEvent()
        {
            PlayOneShot(_generalSFX.continuedInteraction);
        }

        public void PlayEndInteractionEvent()
        {
            PlayOneShot(_generalSFX.endInteraction);
        }

        public void PlayMenuHoverEvent()
        {
            PlayOneShot(_generalSFX.menuHover);
        }

        public void PlayMenuSelectEvent()
        {
            PlayOneShot(_generalSFX.menuSelect);
        }
        #endregion

        #endregion

        #region == [[ BUSES & BANKS ]] ====================================================== >>

        [Header("(( ---- FMOD BANKS ---- ))")]
        [SerializeField, ShowOnly] FMOD.RESULT _bankLoadResult;
        [SerializeField] List<FMODExt_Bank> _bankData;

        [Header("(( ---- FMOD BUSES ---- ))")]
        [SerializeField, ShowOnly] FMOD.RESULT _busLoadResult;
        [SerializeField] List<FMODExt_Bus> _busData;

        #region -- (( LOADING )) ----------------------- ))
        public void LoadBanksAndBuses()
        {
            StartCoroutine(LoadBanksAndBusesRoutine());
        }

        IEnumerator LoadBanksAndBusesRoutine()
        {
            // ------- Load Banks ------- //
            InternalConsole.Log($"{Prefix} Loading Banks.");
            FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] _banks);
            foreach (FMOD.Studio.Bank bank in _banks)
            {
                // Load the bank
                _bankLoadResult = bank.loadSampleData();
                if (_bankLoadResult == FMOD.RESULT.OK)
                {
                    FMODExt_Bank newBankData = new FMODExt_Bank(bank);
                    _bankData.Add(newBankData);
                    InternalConsole.Log($"Bank Load Result: " + newBankData.Path + " -> " + _bankLoadResult, 1);
                }
            }

            // ------- Load Buses ------- //
            InternalConsole.Log($"{Prefix} Loading Buses.");
            foreach (FMODExt_Bank bank in _bankData)
            {
                List<FMODExt_Bus> busData = bank.BusData;
                _busData.AddRange(busData);
                foreach (FMODExt_Bus bus in busData)
                {
                    InternalConsole.Log($"Bus Load Result: " + bus.Path + " -> " + bus.LoadResult, 1);
                }
            }

            yield return null;
        }
        #endregion

        public FMODExt_Bus GetBus(string path)
        {
            return _busData.Find(b => b.Path == path);
        }

        public void SetBusVolume(string path, float volume)
        {
            FMODExt_Bus bus = _busData.Find(b => b.Path == path);
            bus?.SetVolume(volume);
        }

        #endregion

        #region == [[ PLAY EVENT FUNCTIONS ]] =============================================== >>
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
        #endregion

        public static string GetInstantiatedEventPath(EventInstance instance)
        {
            string result;
            EventDescription description;

            instance.getDescription(out description);
            description.getPath(out result);

            // expect the result in the form event:/folder/sub-folder/eventName
            return result;
        }

        // Coroutine to handle the repeated playing of an event
        private IEnumerator RepeatEventRoutine(EventReference eventReference, float interval)
        {
            while (true)
            {
                FMODExt_EventManager.PlayOneShot(eventReference);
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

            // Retrieve the GUID of the current and new song events
            FMOD.GUID currentEventGuid;
            CurrentSongInstance.getDescription(out EventDescription eventDescription);
            eventDescription.getID(out currentEventGuid);

            FMOD.GUID newEventGuid = newSongEventRef.Guid;

            // Compare the GUIDs
            if (currentEventGuid == newEventGuid)
            {
                Debug.LogWarning($"{Prefix} FMOD SONG event is already playing: " + newSongEventRef);
                return;
            }

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
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FMODExt_EventManager), true)]
    public class FMODExt_EventManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        FMODExt_EventManager _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (FMODExt_EventManager)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Draw the consoleGUI in the inspector
            FMODExt_EventManager.InternalConsole.DrawInEditor();

            CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(_serializedObject);

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
