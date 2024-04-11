using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;

[RequireComponent(typeof(OverlapGrid2D))]
public class PlayerUIHandler : MonoBehaviour
{
    OverlapGrid2D overlapGrid2D => GetComponent<OverlapGrid2D>();
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    void Start()
    {
        Coordinate targetGridCoordinate = overlapGrid2D.GetCoordinatesByColliderCount()[0][1];
        CreateDialogueBubbleAt(targetGridCoordinate.worldPosition);
    }

    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition)
    {
        if (activeDialogueBubble == null)
        {
            // Create a new dialogue bubble
            this.activeDialogueBubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
            activeDialogueBubble.transform.SetParent(this.transform);
            activeDialogueBubble.transform.position = worldPosition;
            activeDialogueBubble.Initialize(visualTreeAsset, panelSettings);
        }

        return activeDialogueBubble;
    }

    public void CreateChoices()
    {

    }
}
