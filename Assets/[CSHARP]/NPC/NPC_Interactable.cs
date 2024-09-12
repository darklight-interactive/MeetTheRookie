using UnityEngine;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Utility;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NPC_Controller))]
public class NPC_Interactable : Interactable, IInteract
{

    // ======== [[ FIELDS ]] ================================== >>>>
    NPCState _stateBeforeTalkedTo = NPCState.IDLE;

    public Grid2D_OverlapWeightSpawner dialogueGridSpawner;

    [Dropdown("_speakerOptions")] public string speakerTag;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        // This is just a getter a list of all the speakers in the story
        get
        {
            List<string> speakers = new List<string>();
            if (InkyStoryManager.Instance != null)
            {
                speakers = InkyStoryManager.SpeakerList;
            }
            return speakers;
        }
    }
    NPC_StateMachine _stateMachine => GetComponent<NPC_Controller>().stateMachine;


    // ======== [[ METHODS ]] ================================== >>>>
    public override void Awake()
    {
        base.Awake();
        if (dialogueGridSpawner == null)
        {
            dialogueGridSpawner = GetComponent<Grid2D_OverlapWeightSpawner>();
            if (dialogueGridSpawner == null)
            {

                dialogueGridSpawner = ObjectUtility.InstantiatePrefabWithComponent<Grid2D_OverlapWeightSpawner>
                    (MTR_UIManager.Instance.dialogueSpawnerPrefab, Vector3.zero, Quaternion.identity, transform);
                dialogueGridSpawner.transform.localPosition = Vector3.zero;
            }
        }
    }


    public override void Start()
    {
        SpawnDestinationPoints();

        Reset();

        // >> ON FIRST INTERACTION -------------------------------
        this.OnFirstInteraction += () => 
        {
            _stateBeforeTalkedTo = _stateMachine.CurrentState;

            // If the statemachine is not null, go to the speak state
            _stateMachine?.GoToState(NPCState.SPEAK);
        };

        // >> ON INTERACT ---------------------------------------
        // NOTE :: This event is only called when an Interaction is confirmed
        this.OnInteraction += (string text) =>
        {

        };

        this.OnCompleted += () =>
        {
            // If the statemachine is not null, go to the state before talked to
            _stateMachine?.GoToState(_stateBeforeTalkedTo);
        };
    }

    public void PlayAnimation(NPCState state)
    {
        GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
    }
}