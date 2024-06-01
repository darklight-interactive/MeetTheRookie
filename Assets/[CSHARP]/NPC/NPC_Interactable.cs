using UnityEngine;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NPC_Controller))]
public class NPC_Interactable : Interactable, IInteract
{
    private NPCState stateBeforeTalkedTo = NPCState.IDLE;
    NPC_StateMachine stateMachine => GetComponent<NPC_Controller>().stateMachine;


    [Header("NPC : Speech Bubble")]

    // This is just a getter for the speaker tag options
    private List<string> _speakerOptions
    {
        get
        {
            List<string> speakers = InkyStoryManager.SpeakerList;
            return speakers;
        }
    }

    [Dropdown("_speakerOptions")]
    public string speakerTag;

    public override void Start()
    {
        Reset();

        // >> ON FIRST INTERACTION -------------------------------
        this.OnFirstInteraction += () => 
        {
            stateBeforeTalkedTo = stateMachine.CurrentState;

            // If the statemachine is not null, go to the speak state
            stateMachine?.GoToState(NPCState.SPEAK);
        };

        // >> ON INTERACT ---------------------------------------
        // NOTE :: This event is only called when an Interaction is confirmed
        this.OnInteraction += (string text) =>
        {

        };

        this.OnCompleted += () =>
        {
            // If the statemachine is not null, go to the state before talked to
            stateMachine?.GoToState(stateBeforeTalkedTo);
        };
    }
}