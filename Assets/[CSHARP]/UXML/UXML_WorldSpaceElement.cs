using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(MeshFilter), typeof(MeshRenderer))]
public class UXML_WorldSpaceElement : MonoBehaviour
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root => uiDocument.rootVisualElement;
    MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    MeshFilter meshFilter => GetComponent<MeshFilter>();
    bool initialized;

    UXML_InkyBubble inkyBubble;

    public void Update()
    {

        InkyStoryManager inkyStoryManager = InkyStoryManager.Instance;


        /*
        
        // Update the text only if it has changed
        if (inkyLabel.text != dialogueText)
        {
            inkyLabel.UpdateText(dialogueText);
            if (panelSettings.targetTexture != null)
            {
                panelSettings.targetTexture.Release();

                // destroy the old render texture in play and edit mode
                if (Application.isPlaying)
                {
                    Destroy(panelSettings.targetTexture);
                }
                else
                {
                    DestroyImmediate(panelSettings.targetTexture);
                }
            }

            // create a new render texture
            panelSettings.targetTexture = new RenderTexture(512, 512, 24);

            // assign the render texture to the material
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(material);
            meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        }
        */
    }

    public void ManualUpdate(VisualTreeAsset asset, PanelSettings settings, string text)
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;

        if (settings.targetTexture != null)
            settings.targetTexture.Release();

        // create a new render texture
        //settings.targetTexture = new RenderTexture(UXML_InteractionUI.Instance.worldSpaceRenderTexture);
        settings.targetTexture = new RenderTexture(1080, 1080, 24);

        // Set the material and texture
        Material newMaterial = new Material(UXML_InteractionUI.Instance.worldSpaceMaterial);
        newMaterial.mainTexture = settings.targetTexture;
        meshRenderer.sharedMaterial = newMaterial;

        inkyBubble = root.Q<UXML_InkyBubble>();
        inkyBubble.SetText(text);
        inkyBubble.visible = true;
    }
}
