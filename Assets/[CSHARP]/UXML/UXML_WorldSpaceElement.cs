using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UIDocument))]
public class UXML_WorldSpaceElement : MonoBehaviour
{
    UIDocument uiDocument => GetComponent<UIDocument>();
    VisualElement root;
    MeshRenderer meshRenderer => GetComponentInChildren<MeshRenderer>();
    public UXML_InkyBubble inkyBubble;
    bool initialized = false;

    public void Initialize(VisualTreeAsset asset, PanelSettings settings, Material material, RenderTexture renderTexture)
    {
        uiDocument.visualTreeAsset = asset;
        uiDocument.panelSettings = settings;
        root = uiDocument.rootVisualElement;

        GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshChild.transform.SetParent(this.transform);
        meshChild.transform.localPosition = Vector3.zero;
        meshRenderer.enabled = false;

        // Set the material and texture
        meshRenderer.sharedMaterial = material;

        uiDocument.panelSettings.targetTexture = new RenderTexture(renderTexture);
        meshRenderer.sharedMaterial.mainTexture = uiDocument.panelSettings.targetTexture;

        initialized = true;

        // This is to replace the Update() method with a slower version
        InvokeRepeating("ManualUpdate", 0, 0.1f);
    }

    void ManualUpdate()
    {
        if (initialized == false) return;
        if (uiDocument.panelSettings == null || uiDocument.panelSettings.targetTexture == null) return;


        PanelSettings panelSettings = uiDocument.panelSettings;
        if (panelSettings == null) return;

        inkyBubble = root.Q<UXML_InkyBubble>();

        string currentText = "";
        if (InkyKnotThreader.Instance.currentKnot != null)
        {
            currentText = InkyKnotThreader.Instance.currentStory.currentText;
        }

        if (currentText == null || currentText == "") currentText = "TEXT NOT FOUND";
        inkyBubble.SetText(currentText);
        inkyBubble.visible = true;

        // << #1 -> Reset Material
        meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        // << #2 -> Reset Texture
        panelSettings.targetTexture = new RenderTexture(panelSettings.targetTexture);

        meshRenderer.enabled = true;
    }

    void OnDestroy()
    {
        CancelInvoke("ManualUpdate");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UXML_WorldSpaceElement))]
    public class UXML_WorldSpaceElementEditor : Editor
    {


        void OnDisable()
        {
            UXML_WorldSpaceElement element = (UXML_WorldSpaceElement)target;
            element.OnDestroy();
            DestroyImmediate(element.gameObject);
        }
    }
#endif



}

