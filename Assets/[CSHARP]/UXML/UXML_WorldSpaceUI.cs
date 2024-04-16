using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid;
using Darklight.Console;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.Game.Utility;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class UXML_WorldSpaceUI : MonoBehaviourSingleton<UXML_WorldSpaceUI>
{
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings worldSpacePanelSettings;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;

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
            if (Application.isPlaying)
                Destroy(bubble.gameObject, destroy_after);
            else
                DestroyImmediate(bubble.gameObject); // << for editor
        }

        return bubble;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(UXML_WorldSpaceUI))]
public class UXML_WorldSpaceUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UXML_WorldSpaceUI worldSpaceUI = (UXML_WorldSpaceUI)target;

        if (GUILayout.Button("Create Comic Bubble"))
        {
            worldSpaceUI.CreateComicBubbleAt(worldSpaceUI.transform.position);
        }
    }
}
#endif
