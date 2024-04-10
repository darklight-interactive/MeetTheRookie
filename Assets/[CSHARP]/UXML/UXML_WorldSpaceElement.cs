using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UXML_WorldSpaceElement : MonoBehaviour
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root;
    MeshRenderer meshRenderer => GetComponentInChildren<MeshRenderer>();
    public UXML_InkyBubble inkyBubble;
    bool initialized = false;

    public void Initialize(VisualTreeAsset asset, PanelSettings settings)
    {
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;
        root = uiDocument.rootVisualElement;

        GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshChild.transform.SetParent(this.transform);
        meshChild.transform.localPosition = Vector3.zero;
        meshRenderer.enabled = false;

        // Set the material and texture
        Material worldSpaceMaterial = UXML_InteractionUI.Instance.worldSpaceMaterial;
        meshRenderer.sharedMaterial = worldSpaceMaterial;

        uiDocument.panelSettings.targetTexture = new RenderTexture(UXML_InteractionUI.Instance.worldSpaceRenderTexture);
        meshRenderer.sharedMaterial.mainTexture = uiDocument.panelSettings.targetTexture;

        initialized = true;
    }

    public void Update()
    {
        if (initialized == false) return;

        PanelSettings panelSettings = uiDocument.panelSettings;
        if (panelSettings == null) return;

        inkyBubble = root.Q<UXML_InkyBubble>();
        inkyBubble.SetText(InkyStoryManager.Instance.currentInkDialog.textBody);
        inkyBubble.visible = true;

        // << #1 -> Reset Material
        meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        // << #2 -> Reset Texture
        panelSettings.targetTexture = new RenderTexture(UXML_InteractionUI.Instance.worldSpaceRenderTexture);

        meshRenderer.enabled = true;
    }
}