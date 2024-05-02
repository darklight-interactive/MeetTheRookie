using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class UXML_UIDocumentObject : MonoBehaviour
{
    protected UXML_UIDocumentPreset preset;
    protected UIDocument document => GetComponent<UIDocument>();
    protected VisualElement root => document.rootVisualElement;
    protected Dictionary<string, UXML_UIDocumentElement> uiElements = new Dictionary<string, UXML_UIDocumentElement>();
    protected string[] elementTags = new string[0];

    public virtual void Initialize(UXML_UIDocumentPreset preset, string[] tags)
    {
        this.preset = preset;
        document.visualTreeAsset = preset.VisualTreeAsset;
        document.panelSettings = preset.PanelSettings;

        if (tags != null)
        {
            LoadUIElements(tags);
        }

        gameObject.layer = LayerMask.NameToLayer("UI");
    }

    void LoadUIElements(string[] tags)
    {
        foreach (string tag in tags)
        {
            bool foundElement = AddUIElement(tag);
            if (!foundElement) continue;

            //Debug.Log($"Element with tag {tag} found in UIDocument {document.name}");
        }
    }

    public bool AddUIElement(string tag)
    {
        VisualElement element = root.Query(tag);

        if (element == null)
        {
            Debug.LogError($"Element with tag {tag} not found in UIDocument {document.name}");
            return false;
        }
        uiElements.Add(tag, new UXML_UIDocumentElement(element, tag));
        return true;
    }

    public UXML_UIDocumentElement GetUIElement(string tag)
    {
        if (uiElements.ContainsKey(tag))
        {
            return uiElements[tag];
        }
        return null;
    }

    void OnDestroy()
    {
        foreach (UXML_UIDocumentElement element in uiElements.Values)
        {
            element.visualElement.Clear();
        }
    }
}