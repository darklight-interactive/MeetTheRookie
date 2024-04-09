using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;
using System.Linq;
using Darklight.UnityExt;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(OverlapGrid2D))]
public class NPC_UIHandler : InkyInteraction
{
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public OverlapGrid2D overlapGrid => GetComponent<OverlapGrid2D>();
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    public override void StartInteractionKnot(InkyStoryManager.KnotComplete onComplete)
    {
        base.StartInteractionKnot(() =>
        {
            onComplete?.Invoke();
        });

        Coordinate targetGridCoordinate = overlapGrid.GetCoordinatesByColliderCount()[0][2];
        CreateDialogueBubbleAt(targetGridCoordinate.worldPosition, InkyStoryManager.Instance.currentText);

        Debug.Log($"StartInteractionKnot -> NewDialogueBubble{InkyStoryManager.Instance.currentText}");
    }

    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition, string text)
    {
        // Delete the active dialogue bubble
        if (activeDialogueBubble != null)
            DeleteObject(activeDialogueBubble.gameObject);

        // Create a new dialogue bubble
        this.activeDialogueBubble = GameObject.CreatePrimitive(PrimitiveType.Quad).AddComponent<UXML_WorldSpaceElement>();
        activeDialogueBubble.transform.SetParent(this.transform);
        activeDialogueBubble.transform.position = worldPosition;
        activeDialogueBubble.ManualUpdate(visualTreeAsset, panelSettings, text);
        return activeDialogueBubble;
    }

    public void Update()
    {

        if (activeDialogueBubble != null && visualTreeAsset != null && panelSettings != null)
        {
            if (InkyStoryManager.Instance == null)
                return;
            activeDialogueBubble.ManualUpdate(visualTreeAsset, panelSettings, InkyStoryManager.Instance.currentText);
        }

    }

    public void DeleteObject(GameObject obj)
    {
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}



