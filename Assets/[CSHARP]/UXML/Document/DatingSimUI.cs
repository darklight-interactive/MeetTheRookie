using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Input;
using Darklight.UXML;
using UnityEditor;
using UnityEngine.SceneManagement;
using Darklight.Utility;

public class DatingSimUI : UXML_UIDocumentObject
{
    [Tooltip("Dialogue Text Size Min/Max")] public Vector2 textSize = new Vector2(20, 48);
    [Tooltip("Ink file for this scene")] public TextAsset inkFile;
    public SelectableVectorField<Button> choiceMap = new SelectableVectorField<Button>();
    public SceneAsset sceneAsset;
    // Global variables
    Story currentStory;
    bool choicesActive;

    // Variables for all the visual elements
    VisualElement misraImage;
    VisualElement lupeImage;
    VisualElement continueTriangle;
    VisualElement dialogueBox;
    VisualElement nameTag;
    TextElement dialogueText;
    VisualElement choiceParent;
    List<Button> choiceButtons = new List<Button>(4);

    void Initialize()
    {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }
        UniversalInputManager.OnMoveInput += Move;
        UniversalInputManager.OnPrimaryInteract += Select;
    }

    // Start is called before the first frame update
    void Start()
    {
        // I think this stalls the initialize function
        Invoke("Initialize", 0.1f);

        // Get all the UXML elements
        misraImage = root.Q<VisualElement>("MisraImage");
        lupeImage = root.Q<VisualElement>("LupeImage");

        continueTriangle = root.Q<VisualElement>("DialogueTriangle");
        dialogueBox = root.Q<VisualElement>("DialogueBox");
        nameTag = root.Q<VisualElement>("NameTag");
        dialogueText = root.Q<TextElement>("DialogueText");

        choiceParent = root.Q<VisualElement>("ChoiceParent");
        for (int i = 0; i < choiceButtons.Capacity; i++)
        {
            choiceButtons.Add(root.Q<Button>("Choice" + i));
            choiceMap.Add(choiceButtons[i]);
        }
        // Get Ink story
        currentStory = new Story(inkFile.text);

        // Start story
        ContinueStory();
        MoveTriangle(); // Cool dialogue triangle movement
    }

    /// <summary>
    /// Continues the Story
    /// </summary>
    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            UpdateDialogue(currentStory.Continue());
            if (currentStory.currentChoices.Count > 0)
            {
                PopulateChoices();
            }
        }
        // else if (currentStory.currentChoices.Count > 0)
        // {
        //     PopulateChoices();
        // }
        else if (currentStory.currentChoices.Count <= 0)
        {
            EndStory();
        }
    }

    /// <summary>
    /// Enables the choice buttons
    /// </summary>
    void PopulateChoices()
    {
        choiceParent.style.display = DisplayStyle.Flex;
        continueTriangle.style.visibility = Visibility.Hidden;
        int index = 0;
        foreach (Choice choice in currentStory.currentChoices)
        {
            choiceButtons[index].style.display = DisplayStyle.Flex;
            choiceButtons[index].text = choice.text;
            choiceButtons[index].style.fontSize = textSize.y;
            index++;
        }

        float approxHeight = choiceParent.resolvedStyle.height;
        float approxWidth = choiceParent.resolvedStyle.width / index;
        for (int i = 0; i < index; i++)
        {
            var tempMeasure = choiceButtons[i].MeasureTextSize(choiceButtons[i].text, approxWidth, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            if (tempMeasure.y > approxHeight)
            {
                dialogueText.style.fontSize = Mathf.Min(Mathf.Max(textSize.y * (textSize.y * 3 / approxHeight), textSize.x), textSize.y);
            }
            choiceButtons[i].RemoveFromClassList("Highlight");
        }
        for (int i = index; i < choiceButtons.Count; i++)
        {
            choiceButtons[i].style.display = DisplayStyle.None;
            choiceButtons[i].RemoveFromClassList("Highlight");
        }

        nameTag.AddToClassList("NameTagLupe");
        nameTag.RemoveFromClassList("NameTagMisra");
        lupeImage.RemoveFromClassList("Inactive");
        misraImage.AddToClassList("Inactive");

        choicesActive = true;
        //TODO : Fix this
        //choiceMap.resetSelected(); 
        choiceMap.CurrentSelection.AddToClassList("Highlight");
    }

    /// <summary>
    /// Selects the choice at the given index
    /// </summary>
    /// <param name="index">The index of the choice</param>
    void SelectChoice()
    {
        currentStory.ChooseChoiceIndex(choiceButtons.IndexOf(choiceMap.CurrentSelection));
        choiceParent.style.display = DisplayStyle.None;
        continueTriangle.style.visibility = Visibility.Visible;
        choicesActive = false;
        ContinueStory();
    }

    /// <summary>
    /// Ends the story. Transition to next scene from here.
    /// </summary>
    void EndStory()
    {
        UpdateDialogue("END OF STORY");
        Debug.Log("END OF STORY");
        SceneManager.LoadScene(sceneAsset.name);
    }

    /// <summary>
    /// The function to select choice via input
    /// </summary>
    /// <param name="context">IDK man</param>
    void Select()
    {
        if (choicesActive)
        {
            SelectChoice();
        }
        else
        {
            ContinueStory();
        }
    }

    /// <summary>
    /// The function to change choice via input
    /// </summary>
    /// <param name="context">IDK man</param>
    void Move(Vector2 move)
    {
        move.y = -move.y;
        if (choiceMap.CurrentSelection != null)
        {
            choiceMap.CurrentSelection.RemoveFromClassList("Highlight");
        }
        var selected = choiceMap.getFromDir(move);
        if (selected != null)
        {
            selected.AddToClassList("Highlight");
        }
    }

    /// <summary>
    /// Update the dialogue 
    /// </summary>
    /// <param name="dialogue">The new dialogue</param>
    void UpdateDialogue(string dialogue)
    {
        List<string> tags = currentStory.currentTags;
        nameTag.style.visibility = Visibility.Hidden;
        foreach (string tag in tags)
        {
            string[] splitTag = tag.Split(":");
            if (splitTag[0].Trim() == "name")
            {
                nameTag.style.visibility = Visibility.Visible;
                string name = splitTag[1].Trim();
                if (name == "Lupe")
                {
                    nameTag.AddToClassList("NameTagLupe");
                    nameTag.RemoveFromClassList("NameTagMisra");
                    lupeImage.RemoveFromClassList("Inactive");
                    misraImage.AddToClassList("Inactive");
                }
                else if (name == "Misra")
                {
                    misraImage.style.visibility = Visibility.Visible;
                    nameTag.AddToClassList("NameTagMisra");
                    nameTag.RemoveFromClassList("NameTagLupe");
                    misraImage.RemoveFromClassList("Inactive");
                    lupeImage.AddToClassList("Inactive");
                }
                break;
            }
            // Code for image changeing when it's supported
            /* else if(splitTag[0].Trim()=="Emote"){ // Change with correct tag
                // Find emote
                // Change character based on emote
            } */

        }
        dialogueText.text = dialogue;
        UpdateBoxNTextSize();
    }

    /// <summary>
    /// Updates the text box size and text size
    /// </summary>
    void UpdateBoxNTextSize()
    {
        dialogueText.style.fontSize = textSize.y;
        float width = (!float.IsNaN(dialogueText.resolvedStyle.width)) ? dialogueText.resolvedStyle.width : 1000;
        Vector2 newBoxSize = dialogueText.MeasureTextSize(dialogueText.text, width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
        dialogueBox.style.height = newBoxSize.y * 1.2f;
        dialogueText.style.fontSize = Mathf.Min(Mathf.Max(textSize.y * (textSize.y * 7 / newBoxSize.y), textSize.x), textSize.y);
    }

    /// <summary>
    /// Moves the cool dialogue triangle up and down
    /// </summary>
    void MoveTriangle()
    {
        continueTriangle.ToggleInClassList("TriangleDown");
        continueTriangle.RegisterCallback<TransitionEndEvent>(evt => continueTriangle.ToggleInClassList("TriangleDown"));
        root.schedule.Execute(() => continueTriangle.ToggleInClassList("TriangleDown")).StartingIn(100);
    }
}
