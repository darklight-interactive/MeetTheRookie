using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UXML_InteractionUI : MonoBehaviour, ISceneSingleton<UXML_InteractionUI>
{
    /// <summary>
    /// Convert a world position to a screen position.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public static Vector3 WorldToScreen(Vector3 worldPos)
    {
        if (Camera.main == null) throw new System.Exception("No main camera found.");

        // Per https://forum.unity.com/threads/forcing-a-ui-element-to-follow-a-character-in-3d-space.1010968/
        Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
        pos.y = Camera.main.pixelHeight - pos.y;
        pos.z = 0;
        return pos;
    }

    #region ===== [ UI ELEMENT CLASS ] ==================================================== >>
    /// <summary>
    /// Data structure for an UXML VisualElement.
    /// </summary>
    public class UIElement
    {
        public string tag;
        public VisualElement element;
        public Vector2 screenPosition = Vector2.zero;
        public UIElement(VisualElement element, string tag)
        {
            this.tag = tag;
            this.element = element;
            this.element.visible = false; // << Hide it by default
        }

        public void SetVisible(bool visible)
        {
            element.visible = visible;
        }

        public void SetPosition(Vector3 worldPosition)
        {
            screenPosition = WorldToScreen(worldPosition);
            element.transform.position = screenPosition;
        }
    }

    void AddUIElement(string tag)
    {
        VisualElement element = root.Query(tag);

        if (element == null) throw new System.Exception("No UI element found with tag " + tag);
        uiElements.Add(tag, new UIElement(element, tag));
        Debug.Log("Added UI element with tag " + tag);
    }

    UIElement GetUIElement(string tag)
    {
        if (!uiElements.ContainsKey(tag)) throw new System.Exception("No UI element found with tag " + tag);
        return uiElements[tag];
    }

    UIElement DisplayUIElementAt(string tag, Vector3 worldPosition)
    {
        UIElement uiElement = uiElements[tag];
        if (uiElements == null) throw new System.Exception("No UI element found with tag " + tag);

        uiElement.SetPosition(worldPosition);
        uiElement.SetVisible(true);

        return uiElement;
    }

    void HideUIElement(string tag)
    {
        UIElement uiElement = uiElements[tag];
        if (uiElements == null) throw new System.Exception("No UI element found with tag " + tag);

        uiElement.SetVisible(false);
    }
    #endregion

    string interactPromptTag = "interactPrompt";
    string dialogueBubbleTag = "dialogueBubble";
    string inkyLabelTag = "inkyLabel";

    UIDocument doc;
    VisualElement root;
    Dictionary<string, UIElement> uiElements = new Dictionary<string, UIElement>();

    private void Awake()
    {
        (this as ISceneSingleton<UXML_InteractionUI>).Initialize();

        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        AddUIElement(interactPromptTag);
        AddUIElement(dialogueBubbleTag);
        AddUIElement(inkyLabelTag);

        uiElements[dialogueBubbleTag].element.visible = false;
    }

    public void DisplayInteractPrompt(Vector3 worldPosition)
    {
        DisplayUIElementAt(interactPromptTag, worldPosition);
    }

    public void HideInteractPrompt()
    {
        HideUIElement(interactPromptTag);
    }

    public void DisplayDialogueBubble(Vector3 worldPosition, string text)
    {
        DisplayUIElementAt(dialogueBubbleTag, worldPosition);
        UXML_InkyLabel label = uiElements[inkyLabelTag].element as UXML_InkyLabel;
        label.visible = true;
        label.SetText(text);

        Debug.Log("Displaying dialogue bubble at " + worldPosition + " with text: " + text);
    }

    public void HideDialogueBubble()
    {
        HideUIElement(dialogueBubbleTag);
    }

}
