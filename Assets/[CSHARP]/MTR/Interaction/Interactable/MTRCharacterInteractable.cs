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

public class MTRCharacterInteractable : MTRInteractable
{
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        get
        {
            List<string> speakerList = new List<string>();
            if (MTRStoryManager.SpeakerList != null)
            {
                speakerList = MTRStoryManager.SpeakerList;
            }
            return speakerList;
        }
    }
    public override Type TypeKey => Type.CHARACTER_INTERACTABLE;
    public string SpeakerTag => _speakerTag;


    // ======== [[ METHODS ]] ================================== >>>>
    protected override void PreloadBoxCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = new Vector2(0.5f, 1);
    }

    protected override void GenerateRecievers()
    {
        InteractionSystem.Factory.CreateOrLoadInteractionRequest(TypeKey.ToString(),
            out InteractionRequestDataObject interactionRequest,
            new List<InteractionType> { InteractionType.TARGET, InteractionType.DIALOGUE });
        Request = interactionRequest;
        InteractionSystem.Factory.GenerateInteractableRecievers(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        //Recievers.SetRequiredKeys(Data.InteractionRequest.Keys);


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