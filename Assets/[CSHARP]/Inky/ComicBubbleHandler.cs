using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ComicBubbleHandler : OverlapGrid2D
{
    public string ink_knot { get; set; }
    public Vector3 world_position => transform.position;
    public Darklight.Console console => new Darklight.Console();
    public int counter { get; set; }
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;
    public List<SO_ComicBubble> comicBubbles = new List<SO_ComicBubble>();

    public void Target()
    {
        Vector3? worldPostion = GetBestWorldPosition();
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


    public void CreateBubble()
    {
        Vector3? worldPosition = GetBestWorldPosition();
        if (worldPosition == null)
        {
            Debug.LogError("No best position found. Cant create bubble.");
            return;
        }
        CreateComicBubbleAt((Vector3)worldPosition);
    }

    public Vector3? GetBestWorldPosition()
    {
        Vector2Int? bestPosition = GetBestPositionKey();
        Debug.Log($"GetBestWorldPosition >> {bestPosition}");
        if (bestPosition == null) return null;

        IGrid2DData data = dataGrid.GetData((Vector2Int)bestPosition);
        return data.worldPosition;
    }

    public void CreateChoices()
    {
        if (InkyKnotThreader.Instance.currentStory.currentChoices.Count > 0)
        {
            foreach (Choice choice in InkyKnotThreader.Instance.currentStory.currentChoices)
            {
                IGrid2DData data = dataGrid.GetData(Vector2Int.zero);
                UXML_WorldSpaceElement choiceElement = CreateComicBubbleAt(data.worldPosition, 5f);
            }
        }
    }



}