using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;

public class NPC_Interactable : Interactable
{
    public void Start()
    {
        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += (string currentText) =>
        {
            if (knotIterator.CurrentState == InkyKnotIterator.State.DIALOGUE)
            {
                ShowDialogueBubble(currentText);
            }
        };

        // >> ON COMPLETED -------------------------------------
        this.OnCompleted += () =>
        {
            UIManager.WorldSpaceUI.Hide();
        };
    }

    public UXML_WorldSpaceUI ShowDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.WorldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one;
        //worldSpaceUIDoc.SetText(text);
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