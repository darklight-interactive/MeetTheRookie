using UnityEngine;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NPC_Controller))]
public class NPC_Interactable : Interactable, IInteract
{
    private NPCState stateBeforeTalkedTo = NPCState.NONE;
    NPC_StateMachine stateMachine => GetComponent<NPC_Controller>().stateMachine;


    [Header("NPC : Speech Bubble")]
    [SerializeField, ShowOnly] private GameObject speechBubble;


    public void Start()
    {
        // >> ON FIRST INTERACTION -------------------------------
        this.OnFirstInteraction += () => stateBeforeTalkedTo = stateMachine.CurrentState;

        // >> ON INTERACT ---------------------------------------
        // NOTE :: This event is only called when an Interaction is confirmed
        this.OnInteraction += (string text) =>
        {

        };
    }

    public override void Interact()
    {
        base.Interact();

        if (isComplete) return;
        if (_storyIterator == null) _storyIterator = new InkyStoryIterator(_storyObject);
        if (_storyIterator.CurrentState != InkyStoryIterator.State.END)
        {
            // Create a speech bubble at the best data position
            UIManager.Instance.CreateSpeechBubble(GetBestData().worldPosition, _storyIterator.CurrentText);

            // If the statemachine is not null, go to the speak state
            stateMachine?.GoToState(NPCState.SPEAK);
        }

    }

    public override void Complete()
    {
        base.Complete();

        // Destroy the speech bubble
        UIManager.Instance.DestroySpeechBubble();

        // If the statemachine is not null, go to the state before talked to
        stateMachine?.GoToState(stateBeforeTalkedTo);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPC_Interactable))]
public class InteractableNPCCustomEditor : InteractableCustomEditor
{
    public override void OnInspectorGUI()
    {
        NPC_Interactable interactableNPC = (NPC_Interactable)target;

        base.OnInspectorGUI();
    }
}
#endif