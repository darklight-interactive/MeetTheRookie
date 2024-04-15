using Darklight.Game.Grid;
using UnityEngine;

public class NPCDialogueHandler : Grid2D_OverlapGrid
{

    public void CreateDialogueBubble()
    {
        Vector3 position = this.GetBestData().worldPosition;
        UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(position);
    }

}