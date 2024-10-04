using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.FMODExt;

using Ink.Runtime;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MTRDatingSimController : UXML_UIDocumentObject
{
    const string DIALOGUE_TAG = "dialogue";
    const string CHARACTER_TAG = "character";
    const string CHOICE_TAG = "choice";
    const string CONTAINER_TAG = "container";
    const string IMAGE_TAG = "image";
    const string MISRA_TAG = "misra";
    const string LUPE_TAG = "lupe";


    [SerializeField] bool inCar = false;
    [Tooltip("Next scene to load")] public SceneObject nextScene;
    [SerializeField][Tooltip("Place Dating Sim Emotes Asset Here Please")] private DatingSimEmotes emotes;

    // Inky Variables
    static bool boundEmote = false;


    // Choice Variables
    bool choicesActive;
    SelectableVectorField<SelectableButton> choiceMap = new SelectableVectorField<SelectableButton>();
    bool isRolling = false;

    // UXML Variables
    VisualElement misraImage;
    VisualElement lupeImage;
    VisualElement continueTriangle;
    VisualElement nameTag;
    ControlledLabel dialogueText;
    VisualElement choiceParent;
    List<SelectableButton> choiceButtons = new List<SelectableButton>(4);

    InkyStoryObject _storyObject => InkyStoryManager.GlobalStoryObject;
    InkyStoryIterator _storyIterator => InkyStoryManager.Iterator;

    void CreateTag(List<string> tag_parts, out string outString)
    {
        outString = "";
        foreach (string part in tag_parts)
        {
            if (outString != "") outString += "-";
            outString += part;
        }
    }

    void Awake()
    {
        base.Initialize(preset);

        if (UniversalInputManager.Instance == null)
        {
            Debug.LogError("No Universal Input Manager found in scene. Please add one to the scene.");
            return;
        }
        UniversalInputManager.OnMoveInputStarted += Move;
        UniversalInputManager.OnPrimaryInteract += Select;

        // Get the emotes
        //emotes = Resources.Load<DatingSimEmotes>("ScriptableObjects/DatingSimEmotes");
    }

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<SelectableButton> temp = ElementQueryAll<SelectableButton>();
        choiceButtons = temp.OrderBy(x => x.name).ToList();
        choiceMap.Load(temp);

        // << QUERY UXML ELEMENTS >>
        CreateTag(new List<string> { CHARACTER_TAG, IMAGE_TAG, MISRA_TAG }, out string misraImageTag);
        CreateTag(new List<string> { CHARACTER_TAG, IMAGE_TAG, LUPE_TAG }, out string lupeImageTag);
        misraImage = ElementQuery<VisualElement>(misraImageTag);
        lupeImage = ElementQuery<VisualElement>(lupeImageTag);

        CreateTag(new List<string> { DIALOGUE_TAG, "triangle" }, out string triangleTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag" }, out string nametagTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "text" }, out string dialogueTextTag);
        continueTriangle = ElementQuery<VisualElement>(triangleTag);
        nameTag = ElementQuery<VisualElement>(nametagTag);
        dialogueText = ElementQuery<ControlledLabel>(dialogueTextTag);

        CreateTag(new List<string> { CHOICE_TAG, CONTAINER_TAG, "parent" }, out string choiceParentTag);
        choiceParent = root.Q<VisualElement>(choiceParentTag);

        if (inCar) { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.Flex; }
        else { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.None; }

        choiceParent.style.display = DisplayStyle.None;

        if (!boundEmote)
        {
            // In Inky file function should be: EXTERNAL SetEmote(name, emote)
            _storyObject.BindExternalFunction("SetEmote", (object[] args) =>
            {
                return SetEmote((string)args[0], (string)args[1]);
            });
            boundEmote = true;
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
        Story currentStory = _storyObject.StoryValue;
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
        Story currentStory = _storyObject.StoryValue;
        continueTriangle.style.visibility = Visibility.Hidden;
        int index = 0;
        foreach (Choice choice in currentStory.currentChoices)
        {
            choiceButtons[index].style.display = DisplayStyle.Flex;
            choiceButtons[index].text = choice.text;
            choiceButtons[index].style.fontSize = SetFontSize(false, choice.text);
            choiceButtons[index].Deselect();
            index++;
        }

        for (int i = index; i < choiceButtons.Count; i++)
        {
            choiceButtons[i].style.display = DisplayStyle.None;
            choiceButtons[i].Deselect();
        }

        choicesActive = true;

        //choiceMap.SelectElement(choiceButtons[0]);
        choiceMap.CurrentSelection.SetSelected();
        choiceMap.CurrentSelection.style.fontSize = SetFontSize(true, choiceMap.CurrentSelection.text);
    }

    /// <summary>
    /// Selects the choice at the given index
    /// </summary>
    void SelectChoice()
    {
        Story currentStory = _storyObject.StoryValue;
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
        if (isRolling)
        {
            StopAllCoroutines();
            dialogueText.InstantCompleteText();
            if (choicesActive)
            {
                choiceParent.style.display = DisplayStyle.Flex;
            }
            isRolling = false;
            continueTriangle.visible = true;
        }
        else if (choicesActive)
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
            choiceMap.CurrentSelection.Deselect();
            choiceMap.CurrentSelection.style.fontSize = SetFontSize(false, choiceMap.CurrentSelection.text);
        }
        var selected = choiceMap.GetElementInDirection(move);
        if (selected != null)
        {
            selected.SetSelected();
            selected.style.fontSize = SetFontSize(true, selected.text);
        }
    }

    /// <summary>
    /// Update the dialogue 
    /// </summary>
    /// <param name="dialogue">The new dialogue</param>
    void UpdateDialogue(string dialogue)
    {
        Story currentStory = _storyObject.StoryValue;
        List<string> tags = currentStory.currentTags;
        nameTag.style.visibility = Visibility.Hidden;
        foreach (string tag in tags)
        {
            Debug.Log(tag);
            string[] splitTag = tag.ToLower().Split(":");
            if (splitTag[0].Trim() == "name")
            {
                if (splitTag[1].Trim() == "lupe")
                {
                    nameTag.style.visibility = Visibility.Visible;
                    lupeImage.style.visibility = Visibility.Visible;
                    nameTag.AddToClassList("NameTagLupe");
                    nameTag.RemoveFromClassList("NameTagMisra");
                    lupeImage.RemoveFromClassList("Inactive");
                    misraImage.AddToClassList("Inactive");
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    nameTag.style.visibility = Visibility.Visible;
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
                SetEmote(content[0].Trim(), content[1].Trim());
            }
            else if (splitTag[0].Trim() == "hide")
            {
                if (splitTag[1].Trim() == "lupe")
                {
                    lupeImage.style.visibility = Visibility.Hidden;
                    lupeImage.AddToClassList("Inactive");
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    misraImage.style.visibility = Visibility.Hidden;
                    misraImage.AddToClassList("Inactive");
                }
            }
        }

        StartCoroutine(RollingTextRoutine(dialogue, 0.025f));
    }

    IEnumerator RollingTextRoutine(string fullText, float interval)
    {
        isRolling = true;
        continueTriangle.visible = false;
        dialogueText.SetFullText(fullText); // << Set rolling text
        float buffer = 1f;

        for (int i = 0; i < dialogueText.FullText.Length; i++)
        {
            dialogueText.RollingTextStep();
            buffer -= interval;
            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(Mathf.Max(0, buffer) + 0.25f);
        if (choicesActive)
        {
            choiceParent.style.display = DisplayStyle.Flex;
        }
        isRolling = false;
        continueTriangle.visible = true;
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

    /// <summary>
    /// The manager's function to set the emotes
    /// </summary>
    /// <param name="name"></param>
    /// <param name="emote"></param>
    bool SetEmote(string name, string emote)
    {
        bool success;
        name = name.Trim().ToLower();
        emote = emote.Trim().ToLower();

        success = emotes.SetEmote(name, emote);
        if (name == "lupe")
        {
            FMODExt_EventManager.PlayEventWithParametersByName(emotes.voiceLupeEvent, (emotes.fmodLupeParameterName, emote));
        }
        else if (name == "misra")
        {
            FMODExt_EventManager.PlayEventWithParametersByName(emotes.voiceMisraEvent, (emotes.fmodMisraParameterName, emote));
        }

        return success;
    }

    /// <summary>
    /// HARDCODED: Sets the font size of the choice boxes. Expects max string size of 57.
    /// </summary>
    /// <param name="selected"> If the box is selected or not </param>
    /// <param name="text"> The text to measure to set the font size </param>
    float SetFontSize(bool selected, string text)
    {
        if (selected)
        {
            return Mathf.Pow(5f, Mathf.Clamp((57f - text.Length) / 32f, 0f, 1f) + 1.29f) + 20f;
        }
        else
        {
            return Mathf.Pow(5f, Mathf.Clamp((57f - text.Length) / 32f, 0f, 1f) + 1.115f) + 20f;
        }
    }
}
