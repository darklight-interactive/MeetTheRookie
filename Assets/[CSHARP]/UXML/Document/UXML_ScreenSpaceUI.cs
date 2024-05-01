using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;



/// <summary>
/// Main class for the interaction UI. 
/// This class is responsible for displaying all Screen Space UI 
/// elements related to interaction.
/// </summary>
public class UXML_ScreenSpaceUI : UXML_UIDocumentObject
{
    const string PROMPT_TAG = "interactPrompt";
    const string CHOICE_GROUP_TAG = "choiceGroup";

    [SerializeField] private UXML_UIDocumentPreset _preset;

    public virtual void Awake()
    {
        elementTags = new string[] { PROMPT_TAG, CHOICE_GROUP_TAG };
        Initialize(_preset, elementTags);
    }

    public void DisplayInteractPrompt(Vector3 worldPosition)
    {
        UXML_Element uIElement = GetUIElement(PROMPT_TAG);
        if (uIElement == null) return;
        uIElement.SetWorldToScreenPosition(worldPosition);
        uIElement.SetVisible(true);
    }

    public void HideInteractPrompt()
    {
        UXML_Element uIElement = GetUIElement(PROMPT_TAG);
        if (uIElement == null) return;
        uIElement.SetVisible(false);
    }

    public void CreateChoiceBubble(Vector3 worldPosition, Choice choice)
    {
        UXML_Element groupElement = GetUIElement(CHOICE_GROUP_TAG);
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
