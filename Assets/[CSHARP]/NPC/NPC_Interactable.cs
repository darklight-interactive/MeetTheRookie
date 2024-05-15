using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using UnityEngine.UIElements;
using Darklight.Game.Grid;
using EasyButtons;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPC_Interactable : Interactable, IInteract
{
    private NPCState stateBeforeTalkedTo = NPCState.NONE;
    NPC_Controller controller => GetComponent<NPC_Controller>();


    [Header("NPC : Speech Bubble")]
    [SerializeField, ShowOnly] private GameObject speechBubble;


    public void Start()
    {

        // >> ON FIRST INTERACTION -------------------------------
        this.OnFirstInteraction += FirstInteraction;

        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += Interaction;

        // >> ON COMPLETED -------------------------------------
        this.OnCompleted += Completed;
    }

    public void FirstInteraction()
    {
        stateBeforeTalkedTo = controller.stateMachine.CurrentState;
    }

    public void Interaction(string currentText)
    {
        // Create a speech bubble at the best data position
        UIManager.Instance.CreateSpeechBubble(GetBestData().worldPosition, currentText);
        if (controller)
        {
            controller.stateMachine.GoToState(NPCState.SPEAK);
        }
    }

    public void Completed()
    {
        UIManager.Instance.DestroySpeechBubble();
        if (controller)
        {
            controller.stateMachine.GoToState(stateBeforeTalkedTo);
        }
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