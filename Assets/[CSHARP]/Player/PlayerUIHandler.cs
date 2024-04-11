using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;

public class PlayerUIHandler : OverlapGrid2D
{
    OverlapGrid2D overlapGrid => GetComponent<OverlapGrid2D>();
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    void Start()
    {
        // Create a dialogue bubble at the best position
        Vector2Int bestPosition = overlapGrid.GetOverlapDataWithLowestWeightValue().positionKey;
        Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(bestPosition);
        CreateDialogueBubbleAt(worldPosition);
    }

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

    public void CreateChoices()
    {

    }
}
