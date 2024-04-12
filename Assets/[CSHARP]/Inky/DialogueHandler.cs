using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(OverlapGrid2D))]
public class DialogueHandler : MonoBehaviour
{
    public OverlapGrid2D overlapGrid => GetComponent<OverlapGrid2D>();
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    /// <summary>
    /// Create a dialogue bubble gameobject in the world space at the given position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="destroy_after"></param>
    /// <returns></returns>
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

    public void CreateChoiceBubbleAtBestPosition(Choice choice)
    {
        // Create a dialogue bubble at the best position
        OverlapGrid2D.OverlapData data = overlapGrid.GetDataWithLowestWeightData();
        Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(data.positionKey);
        UXML_InteractionUI.Instance.CreateChoiceBubble(worldPosition, choice);
    }

    public void CreateChoices()
    {
        if (InkyKnotThreader.Instance.currentStory.currentChoices.Count > 0)
        {
            foreach (Choice choice in InkyKnotThreader.Instance.currentStory.currentChoices)
            {
                Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(overlapGrid.GetDataWithLowestWeightData().positionKey);
                UXML_WorldSpaceElement choiceElement = CreateDialogueBubbleAt(worldPosition, 5f);
            }
        }
    }
}