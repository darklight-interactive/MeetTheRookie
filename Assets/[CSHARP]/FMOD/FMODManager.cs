using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Darklight.Game.Utility;

/// <summary>
///  This is the main singleton class that manages all FMOD audio events and buses.
/// </summary>
public class FMODManager : MonoBehaviourSingleton<FMODManager>
{
    // OVERALL VOLUME CONTROL
    [Header("Background Music Event")]
    [SerializeField] private StudioEventEmitter backgroundEmitter;
    public EventReference backgroundMusicEvent;
    public EventInstance backgroundMusicInstance;

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

    private void Start()
    {
        backgroundEmitter = GetComponent<StudioEventEmitter>();

        backgroundMusicInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();
    }


    // Play a single clip through the sound effects source.
    public void Play(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }

}