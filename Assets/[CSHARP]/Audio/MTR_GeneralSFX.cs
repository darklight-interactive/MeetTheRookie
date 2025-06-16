using FMODUnity;
using UnityEngine;


[CreateAssetMenu(menuName = "MeetTheRookie/MTR_GeneralSFX")]
public class MTR_GeneralSFX : FMODExt_SFXObject
{
    [Space(20), Header("---->> MEET THE ROOKIE SPECIFIC SFX")]

    //----------------GENERAL AUDIO----------------//
    [Header("Footstep Audio")]
    [Range(0.1f, 1f)] public float footstepInterval = 0.5f;
    public EventReference footstep;


    [Header("Interaction Audio")]
    public EventReference itemInteract;
    public EventReference paperInteract;
    public EventReference pinpadNumber;
    public EventReference pinpadFail;
    public EventReference pinpadSuccess;

    [Header("Door Audio")]
    public EventReference doorOpen;
    public EventReference doorClose;
    //public EventReference doorLocked;
    //public EventReference doorUnlocked;

    [Header("Miscellaneous Audio")]
    public EventReference phoneDialBeep;
    //public EventReference phoneCallingTone;
    public EventReference phoneVoicemailBeep;
    public EventReference doorBell;
    public EventReference carDoor; // Does this go with "Door Audio"?
    //public EventReference carDrive;


    //----------------SCENE-SPECIFIC AUDIO----------------//
    [Header("Gas Station Audio")]
    public EventReference cashRegister;
    public EventReference sinkSqueak;
    public EventReference fallingTree;

    [Header("Winery Audio")]
    public EventReference misraAndTentacle;
    public EventReference suspiciousNoise;
    //public EventReference cultMotif;


    //----------------DATING SIM AUDIO----------------//
    [Header("Dating Audio")]

    // ROLLING TEXT
    //public EventReference rollingTextDefault;
    //public EventReference rollingTextLupe;
    //public EventReference rollingTextMisra;

    // HUMAN VOICE
    public EventReference voiceLupe;
    public EventReference voiceMisra;
    // String values for parameter changes
    public string parameterNameLupe;
    public string parameterNameMisra;
}