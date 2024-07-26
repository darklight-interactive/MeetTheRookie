using UnityEngine;
using FMODUnity;


[CreateAssetMenu]
public class FMOD_EventReferences : ScriptableObject
{

    //----------------GENERAL AUDIO----------------//
    [Header("Footstep Audio")]
    public EventReference footstepEventReference;
    // Might use more footsteps in the future??

    [Header("Interaction Audio")]
    // Main three
    public EventReference firstInteractionEventReference;
    public EventReference continuedInteractionEventReference;
    public EventReference endInteractionEventReference;
    // Other interactions
    public EventReference itemInteractEventReference;
    public EventReference paperInteractEventReference;

    [Header("UI Audio")]
    public EventReference menuSelectEventReference;
    public EventReference menuHoverEventReference;

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
