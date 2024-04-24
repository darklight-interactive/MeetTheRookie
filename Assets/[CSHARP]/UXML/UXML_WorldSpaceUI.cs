using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class UXML_WorldSpaceUI : UXML_UIDocumentObject
{
    MeshRenderer meshRenderer => GetComponentInChildren<MeshRenderer>();
    private Material _material => UIManager.Instance.worldSpaceMaterial;
    private RenderTexture _renderTexture => UIManager.Instance.worldSpaceRenderTexture;

    // -- Element Tags --
    const string COMIC_BUBBLE_TAG = "comicBubble";

    // -- Element Changed Event --
    public delegate void OnElementChangedDelegate();
    protected OnElementChangedDelegate OnElementChanged;

    private void Awake()
    {
        elementTags = new string[] { COMIC_BUBBLE_TAG };
        Initialize(UIManager.Instance.worldSpaceUIPreset, elementTags);

        // Create a quad mesh child
        GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshChild.transform.SetParent(this.transform);
        meshChild.transform.localPosition = Vector3.zero;
        meshRenderer.enabled = false;
        meshChild.layer = LayerMask.NameToLayer("Player");


        // Begin listening for changes
        OnElementChanged += TextureUpdate;
    }

    public void TextureUpdate()
    {
        // Set the material and texture
        meshRenderer.sharedMaterial = new Material(_material); // << clone the material
        document.panelSettings.targetTexture = new RenderTexture(_renderTexture); // << set UIDocument target texture to clone
        meshRenderer.sharedMaterial.mainTexture = document.panelSettings.targetTexture; // << set the material texture
        meshRenderer.enabled = true;
    }

    public void SetText(string text)
    {
        UXML_ComicBubble comicBubble = root.Q<UXML_ComicBubble>();
        comicBubble.Text = text;
        comicBubble.visible = true;

        // Call the OnChanged event
        OnElementChanged?.Invoke();
    }

    public void Hide()
    {
        UXML_ComicBubble comicBubble = root.Q<UXML_ComicBubble>();
        comicBubble.visible = false;
        meshRenderer.enabled = false;
    }

    public void SetLocalScale(float scale)
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnDestroy()
    {
        OnElementChanged -= TextureUpdate;
    }

}

