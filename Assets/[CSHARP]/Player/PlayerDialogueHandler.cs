using Darklight.Game.Grid;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerDialogueHandler : OverlapGrid2D
{
    private Queue<UXML_WorldSpaceUI> dialogueBubbles = new Queue<UXML_WorldSpaceUI>();
    [SerializeField] private List<UXML_WorldSpaceUI> dialogueBubblesList = new List<UXML_WorldSpaceUI>();

    public UXML_WorldSpaceUI CreateDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.WorldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one;
        worldSpaceUIDoc.SetText(text);
        return worldSpaceUIDoc;
    }

    public void HideDialogueBubble()
    {
        UIManager.WorldSpaceUI.Hide();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDialogueHandler), true)]
public class PlayerDialogueHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Create Dialogue Bubble"))
        {
            PlayerDialogueHandler handler = (PlayerDialogueHandler)target;
            handler.CreateDialogueBubble("Hello World!");
        }
    }
}
#endif

