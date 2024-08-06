using UnityEngine;
using FMODUnity;


[CreateAssetMenu(menuName = "MeetTheRookie/MTR_GeneralSFX")]
public class MTR_GeneralSFX : FMOD_SFXObject
{

    [Space(20), Header("---->> MEET THE ROOKIE SPECIFIC SFX")]

    //----------------GENERAL AUDIO----------------//
    [Header("Footstep Audio")]
    [Range(0.1f, 1f)] public float footstepInterval = 0.5f;
    public EventReference footstep;


    [Header("Interaction Audio")]
    public EventReference itemInteract;
    public EventReference paperInteract;

    [Header("Door Audio")]
    public EventReference doorOpen;
    public EventReference doorClose;
    public EventReference doorLocked;
    public EventReference doorUnlocked;

    [Header("Miscellaneous Audio")]
    public EventReference phoneBeep;
    public EventReference doorBell;
    public EventReference carDoor; // Does this go with "Door Audio"?
    public EventReference carDrive;


    //----------------SCENE-SPECIFIC AUDIO----------------//
    [Header("Gas Station Audio")]
    public EventReference cashRegister;
    public EventReference sinkSqueak;
    public EventReference fallingTree;

    [Header("Winery Audio")]
    public EventReference misraAndTentacle;


    //----------------VOICE ACTING AUDIO----------------//
    [Header("Dating Audio")]
    public EventReference voiceLupe;
    public EventReference voiceMisra;
}