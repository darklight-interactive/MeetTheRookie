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
    public EventReference itemInteractEventReference;
    public EventReference paperInteractEventReference;

    [Header("Door Audio")]
    public EventReference doorOpenEventReference;
    public EventReference doorCloseEventReference;
    public EventReference doorLockedEventReference;
    public EventReference doorUnlockedEventReference;

    [Header("Miscellaneous Audio")]
    public EventReference phoneBeepEventReference;
    public EventReference doorBellEventReference;
    public EventReference carDoorEventReference; // Does this go with "Door Audio"?
    public EventReference carDriveEventReference;


    //----------------SCENE-SPECIFIC AUDIO----------------//
    [Header("Gas Station Audio")]
    public EventReference cashRegisterEventReference;
    public EventReference sinkSqueakEventReference;
    public EventReference fallingTreeEventReference;

    [Header("Winery Audio")]
    public EventReference misraAndTentacleEventReference;


    //----------------VOICE ACTING AUDIO----------------//
    [Header("Dating Audio")]
    public EventReference voiceLupeEventReference;
    public EventReference voiceMisraEventReference;
}
