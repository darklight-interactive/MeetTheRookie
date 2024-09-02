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
            List<string> speakers = new List<string>();
            if (InkyStoryManager.Instance != null)
            {
                speakers = InkyStoryManager.SpeakerList;
            }
            return speakers;
        }
    }

    [Dropdown("_speakerOptions")]
    public string speakerTag;

    public override void Start()
    {
        SpawnDestinationPoints();

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

    public void PlayAnimation(NPCState state)
    {
        GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPC_Interactable))]
public class NPC_InteractableCustomEditor : Editor
{
    SerializedObject _serializedObject;
    NPC_Interactable _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (NPC_Interactable)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Play Animation"))
        {
            _script.PlayAnimation(NPCState.PLAY_ANIMATION);
        }

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif