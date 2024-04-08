using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour, ISceneSingleton<UIManager>
{
    public static Vector3 WorldToScreen(Vector3 worldPos)
    {
        if (Camera.main == null) throw new System.Exception("No main camera found.");

        Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
        // Per https://forum.unity.com/threads/forcing-a-ui-element-to-follow-a-character-in-3d-space.1010968/
        pos.y = Camera.main.pixelHeight - pos.y;
        pos.z = 0;
        return pos;
    }

    UIDocument doc;
    VisualElement root;


    #region ===== [ UI ELEMENT CLASS ] ===== >>
    /// <summary>
    /// A UI Element that can be displayed on the screen.
    /// </summary>
    class UIElement
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

    Dictionary<string, UIElement> uiElements = new Dictionary<string, UIElement>();
    void AddUIElement(string tag)
    {
        VisualElement element = root.Query(tag);
        uiElements.Add(tag, new UIElement(element, tag));
    }

    void DisplayUIElementAt(string tag, Vector3 worldPosition)
    {
        UIElement uiElement = uiElements[tag];
        if (uiElements == null) throw new System.Exception("No UI element found with tag " + tag);

        uiElement.SetPosition(worldPosition);
        uiElement.SetVisible(true);
    }

    void HideUIElement(string tag)
    {
        UIElement uiElement = uiElements[tag];
        if (uiElements == null) throw new System.Exception("No UI element found with tag " + tag);

        uiElement.SetVisible(false);
    }
    #endregion

    private void Awake()
    {
        (this as ISceneSingleton<UIManager>).Initialize();
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        AddUIElement("interactPrompt");
        AddUIElement("dialogueBubble");
    }

    public VisualElement GetUIComponent(string name)
    {
        return root.Query(name);
    }

    public void DisplayInteractPrompt(Vector3 worldPosition)
    {
        DisplayUIElementAt("interactPrompt", worldPosition);
    }

    public void HideInteractPrompt()
    {
        HideUIElement("interactPrompt");
    }

    public void DisplayDialogueBubble(Vector3 worldPosition)
    {
        DisplayUIElementAt("dialogueBubble", worldPosition);
    }

    public void HideDialogueBubble()
    {
        HideUIElement("dialogueBubble");
    }

}
