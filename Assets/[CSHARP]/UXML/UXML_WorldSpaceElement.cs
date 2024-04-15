using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(UIDocument))]
public class UXML_WorldSpaceElement : MonoBehaviour, IUnityEditorListener
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root => uiDocument.rootVisualElement;
    MeshRenderer meshRenderer => GetComponentInChildren<MeshRenderer>();
    public UXMLElement_ComicBubble comicBubble;
    private Material _material;
    private RenderTexture _renderTexture;

    public void Initialize(VisualTreeAsset asset, PanelSettings settings, Material material, RenderTexture renderTexture)
    {
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;
        _material = material;
        _renderTexture = renderTexture;

        // Create a quad mesh child
        GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshChild.transform.SetParent(this.transform);
        meshChild.transform.localPosition = Vector3.zero;

        InvokeRepeating("ManualUpdate", 0.1f, 0.1f);
    }

    public void ManualUpdate()
    {
        comicBubble = root.Q<UXMLElement_ComicBubble>();

        string currentText = "";
        /*
        if (InkyKnotThreader.Instance.currentKnot != null)
        {
            currentText = InkyKnotThreader.Instance.currentStory.currentText;
        }
        */


        if (currentText == null || currentText == "")
        {
            currentText = "TEXT NOT FOUND";
        }

        // << CHECK IF TEXT IS THE SAME >>
        if (comicBubble.Text == currentText)
        {
            return;
        }

        // Update the bubble
        comicBubble.Text = currentText;
        comicBubble.visible = true;

        // Set the material and texture
        meshRenderer.sharedMaterial = new Material(_material); // << clone the material
        uiDocument.panelSettings.targetTexture = new RenderTexture(_renderTexture); // << set UIDocument target texture to clone
        meshRenderer.sharedMaterial.mainTexture = uiDocument.panelSettings.targetTexture; // << set the material texture
        meshRenderer.enabled = true;
    }

    public void SetLocalScale(float scale)
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnDestroy()
    {
        CancelInvoke("ManualUpdate");
    }

    /// <summary>
    /// Called when the editor is reloaded from a script compilation event. Uses the IUnityEditorListener interface.
    /// </summary>
    public void OnEditorReloaded()
    {
        OnDestroy();
        DestroyImmediate(this.gameObject);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UXML_WorldSpaceElement))]
public class UXML_WorldSpaceElementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UXML_WorldSpaceElement element = target as UXML_WorldSpaceElement;

        if (GUILayout.Button("Manual Update"))
        {
            element.ManualUpdate();
        }
    }
}
#endif

