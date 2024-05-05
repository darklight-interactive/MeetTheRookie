using System;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;

public class NPC_Interactable : Interactable
{
    [SerializeField] private float speechBubbleScalar = 1.5f;
    [SerializeField, ShowOnly] private UXML_WorldSpaceUI dialogueBubble;
    public UXML_WorldSpaceUI DialogueBubble { get => dialogueBubble; set => dialogueBubble = value; }

    public void Start()
    {
        NPC_Controller controller = GetComponent<NPC_Controller>();

        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += (string currentText) =>
        {
            dialogueBubble = ShowDialogueBubble(currentText);

            if (controller)
            {
                Debug.Log("Going to SPEAK");
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

    public UXML_WorldSpaceUI ShowDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.Instance.worldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one * speechBubbleScalar;
        //worldSpaceUIDoc.SetText(text);

        worldSpaceUIDoc.TextureUpdate();
        return worldSpaceUIDoc;
    }

    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }
}