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

    [SerializeField] DialogueBubbleSpawner _dialogueHandler;


    NPC_StateMachine _stateMachine => GetComponent<NPC_Controller>().stateMachine;
    public Grid2D_OverlapWeightSpawner DialogueGridSpawner => _dialogueHandler;
    public string speakerTag => _dialogueHandler.speakerTag;


    // ======== [[ METHODS ]] ================================== >>>>
    public override void Awake()
    {
        base.Awake();
        if (_dialogueHandler == null)
        {
            _dialogueHandler = GetComponent<DialogueBubbleSpawner>();
            if (_dialogueHandler == null)
            {

                _dialogueHandler = ObjectUtility.InstantiatePrefabWithComponent<DialogueBubbleSpawner>
                    (MTR_UIManager.Instance.dialogueSpawnerPrefab, Vector3.zero, Quaternion.identity, transform);
                _dialogueHandler.transform.localPosition = Vector3.zero;
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