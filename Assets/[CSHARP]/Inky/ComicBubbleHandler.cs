using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid;
using Darklight.Console;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ComicBubbleHandler : Grid2D_OverlapGrid
{
    public string ink_knot { get; set; }
    public ConsoleGUI console => new ConsoleGUI();
    public int counter { get; set; }
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;
    public List<SO_ComicBubble> comicBubbles = new List<SO_ComicBubble>();

    public void Target()
    {
        //Vector3? worldPostion = GetBestWorldPosition();
        Vector3? worldPostion = null;
        if (worldPostion == null) worldPostion = transform.position;
        UXML_InteractionUI.Instance.DisplayInteractPrompt((Vector3)worldPostion);
    }

    /// <summary>
    /// Create a dialogue bubble gameobject in the world space at the given position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="destroy_after"></param>
    /// <returns></returns>
    private UXML_WorldSpaceElement CreateComicBubbleAt(Vector3 worldPosition, float destroy_after = -1f)
    {
        if (activeDialogueBubble != null)
        {
            if (Application.isPlaying)
                Destroy(activeDialogueBubble.gameObject);
            else
                DestroyImmediate(activeDialogueBubble.gameObject);
        }

        // Create a new dialogue bubble
        this.activeDialogueBubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
        activeDialogueBubble.transform.SetParent(this.transform);
        activeDialogueBubble.transform.position = worldPosition;
        activeDialogueBubble.Initialize(visualTreeAsset, panelSettings, worldSpaceMaterial, worldSpaceRenderTexture);

        if (destroy_after >= 0)
        {
            Destroy(activeDialogueBubble.gameObject, destroy_after);
        }

        return activeDialogueBubble;
    }
}