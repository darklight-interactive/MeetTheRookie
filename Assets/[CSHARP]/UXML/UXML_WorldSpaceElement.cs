using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(MeshFilter), typeof(MeshRenderer))]
public class UXML_WorldSpaceElement : MonoBehaviour
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root;
    MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    MeshFilter meshFilter => GetComponent<MeshFilter>();
    public UXML_InkyBubble inkyBubble;
    bool initialized = false;

    public void Initialize(VisualTreeAsset asset, PanelSettings settings)
    {
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;
        root = uiDocument.rootVisualElement;
        initialized = true;
    }

    public void Update()
    {
        if (initialized == false) return;

        PanelSettings panelSettings = uiDocument.panelSettings;
        inkyBubble = root.Q<UXML_InkyBubble>();
        inkyBubble.SetText(InkyStoryManager.Instance.currentInkDialog.textBody);
        inkyBubble.visible = true;

        // Set the material and texture
        Material worldSpaceMaterial = UXML_InteractionUI.Instance.worldSpaceMaterial;
        worldSpaceMaterial.mainTexture = panelSettings.targetTexture;
        meshRenderer.sharedMaterial = worldSpaceMaterial;
        meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        ResetPanelTexture(panelSettings);

        Debug.Log("world space element update");
    }

    public void ResetPanelTexture(PanelSettings panelSettings)
    {
        if (!initialized || panelSettings == null) return;
        if (panelSettings.targetTexture == null)
        {
            panelSettings.targetTexture = new RenderTexture(1080, 1080, 24);
        }

        panelSettings.targetTexture = new RenderTexture(UXML_InteractionUI.Instance.worldSpaceRenderTexture);
    }
}
