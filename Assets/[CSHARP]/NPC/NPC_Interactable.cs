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
public class NPC_Interactable : Interactable, IInteractable
{
    NPCState _stateBeforeTalkedTo = NPCState.IDLE;
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;
    [SerializeField] DialogueInteractionHandler _dialogueHandler;

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
    public DialogueInteractionHandler DialogueHandler
    {
        get
        {
            if (_dialogueHandler == null)
                _dialogueHandler = GetComponentInChildren<DialogueInteractionHandler>();
            return _dialogueHandler;
        }
        set => _dialogueHandler = value;
    }
    public string SpeakerTag => _speakerTag;


    // ======== [[ METHODS ]] ================================== >>>>
    public override void Awake()
    {
        base.Awake();
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