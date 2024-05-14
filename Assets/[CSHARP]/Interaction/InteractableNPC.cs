using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using UnityEngine.UIElements;
using Darklight.Game.Grid;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(OverlapGrid2D))]
public class InteractableNPC : Interactable, IInteract
{
    [Header("NPC Speech Bubble Settings")]
    [SerializeField] private float speechBubbleScalar = 1.5f;
    [SerializeField, ShowOnly] private UXML_WorldSpaceUI dialogueBubble;
    public UXML_WorldSpaceUI DialogueBubble { get => dialogueBubble; set => dialogueBubble = value; }
    public OverlapGrid2D overlapGrid2D => GetComponent<OverlapGrid2D>();

    public void Start()
    {
        NPC_Controller controller = GetComponent<NPC_Controller>();

        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += (string currentText) =>
        {
            //dialogueBubble = ShowDialogueBubble(currentText);
            if (controller)
            {
                controller.stateMachine.GoToState(NPCState.SPEAK);
            }
        };

        // >> ON COMPLETED -------------------------------------
        this.OnCompleted += () =>
        {
            //UIManager.WorldSpaceUI.Hide();
            if (controller)
            {
                controller.stateMachine.GoToState(NPCState.IDLE);
            }
        };
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractableNPC))]
public class InteractableNPCCustomEditor : InteractableCustomEditor
{
    public override void OnInspectorGUI()
    {
        InteractableNPC interactableNPC = (InteractableNPC)target;

        DrawDefaultInspector();

    }
}
#endif