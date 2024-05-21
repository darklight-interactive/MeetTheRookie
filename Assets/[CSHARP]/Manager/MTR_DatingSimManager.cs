using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Input;
using Darklight.UXML.Element;
using UnityEngine.SceneManagement;
using Darklight.Utility;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using System.Linq;

public class MTR_DatingSimManager : UXML_UIDocumentObject
{
    [Tooltip("Dialogue Text Size Min/Max")] public Vector2 textSize = new Vector2(20, 48);
    [Tooltip("Next scene to load")] public SceneObject nextScene;
    [Tooltip("The Dating Sim Story Object")] public InkyStoryObject storyObject;
    [SerializeField] private DatingSimEmotes emotes;

    // Global variables
    bool choicesActive;
    // The Field to navigate buttons
    SelectableVectorField<SelectableButton> choiceMap = new SelectableVectorField<SelectableButton>();
    // Variables for all the visual elements
    VisualElement misraImage;
    VisualElement lupeImage;
    VisualElement continueTriangle;
    VisualElement dialogueBox;
    VisualElement nameTag;
    TextElement dialogueText;
    VisualElement choiceParent;
    List<SelectableButton> choiceButtons = new List<SelectableButton>(4);

    public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
    {
        base.Initialize(preset, tags);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }
        UniversalInputManager.OnMoveInputStarted += Move;
        UniversalInputManager.OnPrimaryInteract += Select;

        // Get the emotes
        emotes = Resources.Load<DatingSimEmotes>("ScriptableObjects/DatingSimEmotes");

        var temp = ElementQueryAll<SelectableButton>();
        choiceButtons = temp.OrderBy(x => x.name).ToList();
        choiceMap.Load(temp);


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
            choiceButtons.Add(root.Q<SelectableButton>("Choice" + i));
            choiceMap.Add(choiceButtons[i]);
        }

        // Start story
        ContinueStory();
        MoveTriangle(); // Cool dialogue triangle movement
    }

    private void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= Move;
        UniversalInputManager.OnPrimaryInteract -= Select;
    }

    /// <summary>
    /// Continues the Story
    /// </summary>
    void ContinueStory()
    {
        Story currentStory = storyObject.story;
        if (currentStory.canContinue)
        {
            UpdateDialogue(currentStory.Continue());
            if (currentStory.currentChoices.Count > 0)
            {
                PopulateChoices();
            }
        }
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
        Story currentStory = storyObject.story;
        choiceParent.style.display = DisplayStyle.Flex;
        continueTriangle.style.visibility = Visibility.Hidden;
        int index = 0;
        foreach (Choice choice in currentStory.currentChoices)
        {
            choiceButtons[index].style.display = DisplayStyle.Flex;
            choiceButtons[index].text = choice.text;
            choiceButtons[index].style.fontSize = textSize.y;
            choiceButtons[index].RemoveFromClassList("Highlight");
            index++;
        }

        // float approxHeight = choiceParent.resolvedStyle.height;
        // float approxWidth = choiceParent.resolvedStyle.width / index;
        // for (int i = 0; i < index; i++)
        // {
        //     var tempMeasure = choiceButtons[i].Text.MeasureTextSize(choiceButtons[i].Text, approxWidth, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
        //     if (tempMeasure.y > approxHeight)
        //     {
        //         dialogueText.style.fontSize = Mathf.Min(Mathf.Max(textSize.y * (textSize.y * 3 / approxHeight), textSize.x), textSize.y);
        //     }
        //     choiceButtons[i].RemoveFromClassList("Highlight");
        // }
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

        choiceMap.Select(choiceButtons[0]);
        choiceMap.CurrentSelection.AddToClassList("Highlight");
    }

    /// <summary>
    /// Selects the choice at the given index
    /// </summary>
    void SelectChoice()
    {
        Story currentStory = storyObject.story;
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
        SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// The function to select choice via input
    /// </summary>
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
    /// <param name="move">The movement vector</param>
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
        Story currentStory = storyObject.story;
        List<string> tags = currentStory.currentTags;
        nameTag.style.visibility = Visibility.Hidden;
        foreach (string tag in tags)
        {
            Debug.Log(tag);
            string[] splitTag = tag.ToLower().Split(":");
            if (splitTag[0].Trim() == "name")
            {
                nameTag.style.visibility = Visibility.Visible;
                if (splitTag[1].Trim() == "lupe")
                {
                    lupeImage.style.visibility = Visibility.Visible;
                    nameTag.AddToClassList("NameTagLupe");
                    nameTag.RemoveFromClassList("NameTagMisra");
                    lupeImage.RemoveFromClassList("Inactive");
                    misraImage.AddToClassList("Inactive");
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    misraImage.style.visibility = Visibility.Visible;
                    nameTag.AddToClassList("NameTagMisra");
                    nameTag.RemoveFromClassList("NameTagLupe");
                    misraImage.RemoveFromClassList("Inactive");
                    lupeImage.AddToClassList("Inactive");
                }
            }
            else if (splitTag[0].Trim() == "emote")
            {
                string[] content = splitTag[1].Split("|");
                emotes.SetEmote(content[0].Trim(), content[1].Trim());
            }
            else if (splitTag[0].Trim() == "hide")
            {
                if (splitTag[1].Trim() == "lupe")
                {
                    lupeImage.style.visibility = Visibility.Hidden;
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    misraImage.style.visibility = Visibility.Hidden;
                }
            }
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
        float trueBoxHeight = (dialogueBox.style.height.value.value > 223f) ? 190f : 170f;
        Debug.Log("Height: " + dialogueText.resolvedStyle.height + "; Adjusted: " + newBoxSize.y);
        dialogueText.style.fontSize = Mathf.Max(textSize.y * Mathf.Clamp(trueBoxHeight / newBoxSize.y, 0, 1), textSize.x);
        Debug.Log(dialogueText.style.fontSize);
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
