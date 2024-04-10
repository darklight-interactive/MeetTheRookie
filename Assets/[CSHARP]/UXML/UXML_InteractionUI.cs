using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



/// <summary>
/// <<<
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UXML_InteractionUI : MonoBehaviour, ISceneSingleton<UXML_InteractionUI>
{
    public static UXML_InteractionUI Instance => ISceneSingleton<UXML_InteractionUI>.Instance;

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
    public class UXML_Element
    {
        public string tag;
        public VisualElement visualElement;
        public Vector2 screenPosition = Vector2.zero;
        public UXML_Element(VisualElement element, string tag)
        {
            this.tag = tag;
            this.visualElement = element;
            this.visualElement.visible = false; // << Hide it by default
        }

        public void SetVisible(bool visible)
        {
            visualElement.visible = visible;
        }

        public void SetWorldToScreenPosition(Vector3 worldPosition)
        {
            screenPosition = WorldToScreen(worldPosition);
            visualElement.transform.position = screenPosition;
        }
    }

    void AddUIElement(string tag)
    {
        VisualElement element = root.Query(tag);

        if (element == null) throw new System.Exception("No UI element found with tag " + tag);
        uiElements.Add(tag, new UXML_Element(element, tag));
    }

    public UXML_Element GetUIElement(string tag)
    {
        if (!uiElements.ContainsKey(tag)) throw new System.Exception("No UI element found with tag " + tag);
        return uiElements[tag];
    }
    #endregion

    string interactPromptTag = "interactPrompt";
    string choiceGroupTag = "choiceGroup";

    UIDocument doc;
    VisualElement root;
    Dictionary<string, UXML_Element> uiElements = new Dictionary<string, UXML_Element>();

    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;

    private void Awake()
    {
        (this as ISceneSingleton<UXML_InteractionUI>).Initialize();

        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        AddUIElement(interactPromptTag);
        AddUIElement(choiceGroupTag);

        uiElements[choiceGroupTag].visualElement.visible = false;
    }

    public void DisplayInteractPrompt(Vector3 worldPosition)
    {
        UXML_Element uIElement = GetUIElement(interactPromptTag);
        uIElement.SetVisible(true);
        uIElement.SetWorldToScreenPosition(worldPosition);
    }

    public void HideInteractPrompt()
    {
        UXML_Element uIElement = GetUIElement(interactPromptTag);
        uIElement.SetVisible(false);
    }
    /*
        public void MoveUpdate(Vector2 move)
        {
            if (!handlingChoice)
            {
                return;
            }
            float x = Mathf.Sign(move.x);
            float y = -Mathf.Sign(move.y);

            int choice = activeChoice;
            if (Mathf.Abs(move.x) > 0.05f)
            {
                choice = (int)Mathf.Clamp(activeChoice + x, 0, story.currentChoices.Count - 1);
            }
            else if (Mathf.Abs(move.y) > 0.05f)
            {
                choice = (int)Mathf.Clamp(activeChoice + y, 0, story.currentChoices.Count - 1);
            }
            UpdateActiveChoice(choice);
        }
    */

}
