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
public class NPC_Interactable : MTR_Interactable, IInteractable
{
    public new const string PREFIX = "<NPC>";

    NPCState _stateBeforeTalkedTo = NPCState.IDLE;
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;
    [SerializeField] DialogueInteractionReciever _dialogueHandler;

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
    public DialogueInteractionReciever DialogueHandler
    {
        get
        {
            if (_dialogueHandler == null)
                _dialogueHandler = GetComponentInChildren<DialogueInteractionReciever>();
            return _dialogueHandler;
        }
        set => _dialogueHandler = value;
    }
    public string SpeakerTag => _speakerTag;


    // ======== [[ METHODS ]] ================================== >>>>
    public override void Preload()
    {
        base.Preload();

        InteractionSystem.Factory.CreateOrLoadRequestPreset(out InteractionRequestPreset interactionRequest, "NPC_InteractionRequestPreset");
        Data.SetInteractionRequest(interactionRequest);

    }

    public override void Initialize()
    {
        base.Initialize();
        Recievers.SetRequiredKeys(Data.InteractionRequest.Keys);


        SpawnDestinationPoints();

        Reset();

        /*
            // >> ON FIRST INTERACTION -------------------------------
            this.OnStartInteraction += () =>
            {
                _stateBeforeTalkedTo = _stateMachine.CurrentState;

                // If the statemachine is not null, go to the speak state
                _stateMachine?.GoToState(NPCState.SPEAK);
            };

            // >> ON INTERACT ---------------------------------------
            // NOTE :: This event is only called when an Interaction is confirmed
            this.OnContinueInteraction += (string text) =>
            {

            };

            this.OnCompleteInteraction += () =>
            {
                // If the statemachine is not null, go to the state before talked to
                _stateMachine?.GoToState(_stateBeforeTalkedTo);
            };
            */
    }

    public void PlayAnimation(NPCState state)
    {
        GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
    }
}