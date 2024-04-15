using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(UIDocument))]
public class UXML_WorldSpaceElement : MonoBehaviour
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root => uiDocument.rootVisualElement;
    MeshRenderer meshRenderer => GetComponentInChildren<MeshRenderer>();
    public UXMLElement_ComicBubble inkyBubble;
    bool initialized = false;

    public void Initialize(VisualTreeAsset asset, PanelSettings settings, Material material, RenderTexture renderTexture)
    {
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;

        // Create a quad mesh child
        GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshChild.transform.SetParent(this.transform);
        meshChild.transform.localPosition = Vector3.zero;
        meshRenderer.enabled = false;

        // Set the material and texture
        meshRenderer.sharedMaterial = new Material(material);
        uiDocument.panelSettings.targetTexture = new RenderTexture(renderTexture);
        meshRenderer.sharedMaterial.mainTexture = uiDocument.panelSettings.targetTexture;

        initialized = true;

        InvokeRepeating("ManualUpdate", 0.1f, 0.1f);
    }

    public void ManualUpdate()
    {
        if (initialized == false) return;
        if (uiDocument.panelSettings == null || uiDocument.panelSettings.targetTexture == null) return;

        PanelSettings panelSettings = uiDocument.panelSettings;
        if (panelSettings == null) return;

        inkyBubble = root.Q<UXMLElement_ComicBubble>();

        string currentText = "";
        /*
        if (InkyKnotThreader.Instance.currentKnot != null)
        {
            currentText = InkyKnotThreader.Instance.currentStory.currentText;
        }
        */

        if (currentText == null || currentText == "") currentText = "TEXT NOT FOUND";
        inkyBubble.SetText(currentText);
        inkyBubble.visible = true;

        // << #1 -> Reset Material
        meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        // << #2 -> Reset Texture
        panelSettings.targetTexture = new RenderTexture(panelSettings.targetTexture);

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

