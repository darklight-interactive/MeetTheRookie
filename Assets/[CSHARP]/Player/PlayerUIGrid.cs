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
}

