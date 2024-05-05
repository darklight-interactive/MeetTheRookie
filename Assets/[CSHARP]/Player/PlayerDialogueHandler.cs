using Darklight.Game.Grid;
using UnityEngine;
using Darklight.UnityExt.UXML;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerDialogueHandler : OverlapGrid2D
{
    public UXML_WorldSpaceUI CreateDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.Instance.worldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one;
        //worldSpaceUIDoc.SetText(text);
        return worldSpaceUIDoc;
    }

    public void HideDialogueBubble()
    {
        UIManager.Instance.worldSpaceUI.Hide();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDialogueHandler), true)]
public class PlayerDialogueHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif

