using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid;
using Darklight.Console;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UXML_WorldSpaceUI : MonoBehaviour, ISceneSingleton<UXML_WorldSpaceUI>
{
    public static UXML_WorldSpaceUI Instance => ISceneSingleton<UXML_WorldSpaceUI>.Instance;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings worldSpacePanelSettings;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;

    void Awake()
    {
        (this as ISceneSingleton<UXML_WorldSpaceUI>).Initialize();
    }

    /// <summary>
    /// Create a dialogue bubble gameobject in the world space at the given position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="destroy_after"></param>
    /// <returns></returns>
    public UXML_WorldSpaceElement CreateComicBubbleAt(Vector3 worldPosition, float destroy_after = -1f)
    {
        // Create a new dialogue bubble
        UXML_WorldSpaceElement bubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
        bubble.transform.SetParent(this.transform);
        bubble.transform.position = worldPosition;
        bubble.Initialize(visualTreeAsset, worldSpacePanelSettings, worldSpaceMaterial, worldSpaceRenderTexture);

        if (destroy_after >= 0)
        {
            Destroy(bubble.gameObject, destroy_after);
        }

        return bubble;
    }
}