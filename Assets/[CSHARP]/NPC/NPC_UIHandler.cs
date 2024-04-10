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

    public override void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        base.StartInteractionKnot(() =>
        {
            onComplete?.Invoke();
        });

        Coordinate targetGridCoordinate = overlapGrid.GetCoordinatesByColliderCount()[0][1];

        InkyDecryptor currentDialogue = new InkyDecryptor(InkyKnotThreader.Instance.currentText);
        CreateDialogueBubbleAt(targetGridCoordinate.worldPosition, currentDialogue.textBody);

        Debug.Log($"StartInteractionKnot -> NewDialogueBubble{currentDialogue.textBody}");
    }

    public override void ResetInteraction()
    {
        base.ResetInteraction();

        DeleteObject(this.activeDialogueBubble.gameObject);
    }

    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition, string text)
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

    public void DeleteObject(GameObject obj)
    {
        if (Application.isPlaying)
            Destroy(obj);
        else
#if UNITY_EDITOR
            DestroyImmediate(obj);
#endif
    }
}



