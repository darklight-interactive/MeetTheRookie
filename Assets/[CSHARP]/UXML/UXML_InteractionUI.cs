using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;



/// <summary>
/// <<<
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UXML_InteractionUI : MonoBehaviourSingleton<UXML_InteractionUI>
{
    string interactPromptTag = "interactPrompt";
    string choiceGroupTag = "choiceGroup";

    UIDocument doc;
    VisualElement root;
    Dictionary<string, UXML_Element> uiElements = new Dictionary<string, UXML_Element>();

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

    private void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        AddUIElement(interactPromptTag);
        AddUIElement(choiceGroupTag);

        //uiElements[choiceGroupTag].visualElement.visible = false;
    }

    public void DisplayInteractPrompt(Vector3 worldPosition)
    {
        UXML_Element uIElement = GetUIElement(interactPromptTag);
        uIElement.SetWorldToScreenPosition(worldPosition);
        uIElement.SetVisible(true);
    }

    public void HideInteractPrompt()
    {
        UXML_Element uIElement = GetUIElement(interactPromptTag);
        uIElement.SetVisible(false);
    }

    public void CreateChoiceBubble(Vector3 worldPosition, Choice choice)
    {
        UXML_Element groupElement = GetUIElement(choiceGroupTag);
        groupElement.SetVisible(true);
        groupElement.SetWorldToScreenPosition(worldPosition);

        Label newLabel = new Label(choice.text);
        newLabel.AddToClassList("inky-label");
        newLabel.AddToClassList("inky-choice__unselected");
        newLabel.visible = true;
        groupElement.visualElement.Add(newLabel);

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
