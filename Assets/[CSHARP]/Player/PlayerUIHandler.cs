using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(OverlapGrid2D))]
public class PlayerUIHandler : MonoBehaviour
{
    public OverlapGrid2D overlapGrid => GetComponent<OverlapGrid2D>();
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition, float destroy_after = -1f)
    {
        if (activeDialogueBubble == null)
        {
            // Create a new dialogue bubble
            this.activeDialogueBubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
            activeDialogueBubble.transform.SetParent(this.transform);
            activeDialogueBubble.transform.position = worldPosition;
            activeDialogueBubble.Initialize(visualTreeAsset, panelSettings);
        }

        if (destroy_after >= 0)
        {
            Destroy(activeDialogueBubble.gameObject, destroy_after);
        }

        return activeDialogueBubble;
    }

    public void CreateBubbleAtBestPosition()
    {
        // Create a dialogue bubble at the best position
        OverlapGrid2D.OverlapData data = overlapGrid.GetDataWithLowestWeightData();
        Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(data.positionKey);
        CreateDialogueBubbleAt(worldPosition);
    }

    public void CreateChoices()
    {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerUIHandler))]
    public class PlayerUIHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PlayerUIHandler playerUIHandler = (PlayerUIHandler)target;
            if (GUILayout.Button("Create Bubble At Best Position"))
            {
                playerUIHandler.CreateBubbleAtBestPosition();
            }
            if (GUILayout.Button("Create Choices"))
            {
                playerUIHandler.CreateChoices();
            }
        }
    }
#endif

}