using Darklight.Game.Grid;
using UnityEngine;
using Darklight.UXML;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerUIGrid : OverlapGrid2D
{
    public void NewSpeechBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UIManager.Instance.ShowSpeechBubbleInWorld(position, text);
        Debug.Log($"Player Dialogue -> Grid Position: {data.positionKey} | World Position: {data.worldPosition}");
    }


    public void HideDialogueBubble()
    {
        UIManager.Instance.worldSpaceUI.Hide();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerUIGrid), true)]
public class PlayerDialogueHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        PlayerUIGrid handler = (PlayerUIGrid)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("New Dialogue Bubble"))
        {
            handler.NewSpeechBubble("Hello World!");
        }
    }
}
#endif

