using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum SoundState { MENU, ACTIVE_MUSIC }

public class FMOD_AudioManager : MonoBehaviour
{
    private GameManager gameManager;
    public Transform playerTransform;

    [Space(10)]
    public SoundState state = SoundState.ACTIVE_MUSIC;

    // OVERALL VOLUME CONTROL


    [Header("Background Music Event")]
    public StudioEventEmitter backgroundEmitter;
    public EventReference backgroundMusicEvent;
    public EventInstance backgroundMusicInstance;

    [Header("Game State")]
    public int musicIntensity;      // the current music intensity
    public bool deathMarch;
    public bool lifeFlowerHealed;

    [Space(10)]
    public LayerMask enemyLayer;    // the layer where enemies are located
    public float outerDetectionRadius = 50;   // the radius of the sphere used to detect enemies
    public float innerDetectionRadius = 25;
    List<Collider2D> outerDetectionOverlap;
    List<Collider2D> innerDetectionOverlap;

    [Space(10)]
    public float curEntityIntensity = 0;
    public int maxEntityIntensity = 5;      // the maximum number of entities that can be in the sphere
    public float closestDistance;   // the closest distance to an enemy collider

    [Header("FMOD Parameters")]
    public float thatManProximity;
    FMOD.Studio.PARAMETER_DESCRIPTION thatManProximityParameter;
    public float leviathanProximity;
    public float lifeFlowerProximity;

    [Header("ONE SHOT FMOD EVENTS")]
    public string lightPickupSound = "event:/lightPickup";

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();


        backgroundMusicInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();

    }


    // Play a single clip through the sound effects source.
    public void Play(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }

}