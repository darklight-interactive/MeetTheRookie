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

public partial class MTRDatingSimController : UXML_UIDocumentObject
{
    const string PREFIX = "<MTRDatingSimController>";
    const string DIALOGUE_TAG = "dialogue";
    const string CHARACTER_TAG = "character";
    const string CHOICE_TAG = "choice";
    const string CONTAINER_TAG = "container";
    const string IMAGE_TAG = "image";
    const string MISRA_TAG = "misra";
    const string LUPE_TAG = "lupe";

    static bool boundEmote = false;

    bool _choicesActive;
    bool _isRolling = false;
    [SerializeField, ShowOnly] bool _inputEnabled = false;
    VisualElement _misraImage;
    VisualElement _lupeImage;
    VisualElement _continueTriangle;
    VisualElement _nameTag;
    ControlledLabel _dialogueText;
    VisualElement _choiceParent;
    List<SelectableButton> _choiceButtons = new List<SelectableButton>(4);
    [SerializeField] SelectableVectorField<SelectableButton> _choiceMap = new SelectableVectorField<SelectableButton>();

    public bool inCar = false;
    [Tooltip("Next scene to load")] public SceneObject nextScene;
    [SerializeField][Tooltip("Place Dating Sim Emotes Asset Here Please")] private DatingSimEmotes emotes;

    // ================ [[ PROPERTIES ]] ================================ >>>>
    InkyStoryIterator _storyIterator => InkyStoryManager.Iterator;

    // ================ [[ UNITY METHODS ]] ============================ >>>>
    void OnEnable()
    {
        MTRSceneController.StateMachine.OnStateChanged += OnSceneStateChanged;
    }
    void OnDestroy()
    {
        SetInputEnabled(false); // Unbind input events
        MTRSceneController.StateMachine.OnStateChanged += OnSceneStateChanged;
    }
    // ================ [[ INTERNAL METHODS ]] ========================== >>>>
    void OnSceneStateChanged(MTRSceneState newState)
    {
        switch (newState)
        {
            case MTRSceneState.ENTER:
                Initialize();
                break;
        }
    }

    void Initialize()
    {
        Debug.Log($"{PREFIX} >> Initialize");
        base.Initialize(preset);

        // << DISABLE INPUTS >>
        SetInputEnabled(false);

        // << QUERY SELECTABLE BUTTONS >>
        IEnumerable<SelectableButton> temp = ElementQueryAll<SelectableButton>();
        _choiceButtons = temp.OrderBy(x => x.name).ToList();
        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            _choiceButtons[i].name = $"choice-button-{i}";
            _choiceButtons[i].Deselect();
            _choiceButtons[i].SetVisible(false);
        }
        Debug.Log($"{PREFIX} >> Choice Buttons: {_choiceButtons.Count}");

        // << QUERY UXML ELEMENTS >>
        CreateTag(new List<string> { CHARACTER_TAG, IMAGE_TAG, MISRA_TAG }, out string misraImageTag);
        CreateTag(new List<string> { CHARACTER_TAG, IMAGE_TAG, LUPE_TAG }, out string lupeImageTag);
        _misraImage = ElementQuery<VisualElement>(misraImageTag);
        _lupeImage = ElementQuery<VisualElement>(lupeImageTag);

        CreateTag(new List<string> { DIALOGUE_TAG, "triangle" }, out string triangleTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag" }, out string nametagTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "text" }, out string dialogueTextTag);
        _continueTriangle = ElementQuery<VisualElement>(triangleTag);
        _nameTag = ElementQuery<VisualElement>(nametagTag);
        _dialogueText = ElementQuery<ControlledLabel>(dialogueTextTag);

        CreateTag(new List<string> { CHOICE_TAG, CONTAINER_TAG, "parent" }, out string choiceParentTag);
        _choiceParent = root.Q<VisualElement>(choiceParentTag);

        if (inCar) { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.Flex; }
        else { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.None; }

        if (!boundEmote)
        {
            // In Inky file function should be: EXTERNAL SetEmote(name, emote)
            InkyStoryManager.GlobalStoryObject.BindExternalFunction("SetEmote", (object[] args) =>
            {
                return SetEmote((string)args[0], (string)args[1]);
            });
            boundEmote = true;
        }

        _dialogueText.FullText = "";

        _storyIterator.OnDialogue += UpdateDialogue;
        _storyIterator.OnChoice += PopulateChoices;

        // Start story
        ContinueStory();
        MoveTriangle(); // Cool dialogue triangle movement

        // << ENABLE INPUTS >>
        SetInputEnabled(true);
    }

    void CreateTag(List<string> tag_parts, out string outString)
    {
        outString = "";
        foreach (string part in tag_parts)
        {
            if (outString != "") outString += "-";
            outString += part;
        }
    }

    void SetInputEnabled(bool enabled)
    {
        switch (enabled)
        {
            case true:
                UniversalInputManager.OnMoveInputStarted += HandleSelectionInput;
                UniversalInputManager.OnPrimaryInteract += Select;
                _inputEnabled = true;
                break;
            case false:
                UniversalInputManager.OnMoveInputStarted -= HandleSelectionInput;
                UniversalInputManager.OnPrimaryInteract -= Select;
                _inputEnabled = false;
                break;
        }
    }

    /// <summary>
    /// Continues the Story
    /// </summary>
    void ContinueStory()
    {
        Debug.Log($"{PREFIX} >> Continue Story");

        StartCoroutine(ContinueStoryRoutine());
    }

    IEnumerator ContinueStoryRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _storyIterator.ContinueStory();

        /*
        Story currentStory = _storyObject.StoryValue;
        if (currentStory.canContinue)
        {
            UpdateDialogue(currentStory.Continue());

            yield return new WaitForSeconds(0.5f); // << Wait for a bit before showing choices
            if (currentStory.currentChoices.Count > 0)
            {
                PopulateChoices();
            }
        }
        else if (currentStory.currentChoices.Count <= 0)
        {
            EndStory();
        }
        */
    }

    #region ======== [[ STORY DIALOGUE ]] <PRIVATE_METHODS> ========================== >>>>
    /// <summary>
    /// Update the dialogue 
    /// </summary>
    /// <param name="dialogue">The new dialogue</param>
    void UpdateDialogue(string dialogue, string speaker = "")
    {
        _storyIterator.TryGetTags(out IEnumerable<string> tags);

        _nameTag.style.visibility = Visibility.Hidden;
        foreach (string tag in tags)
        {
            Debug.Log(tag);
            string[] splitTag = tag.ToLower().Split(":");
            if (splitTag[0].Trim() == "name")
            {
                if (splitTag[1].Trim() == "lupe")
                {
                    _nameTag.style.visibility = Visibility.Visible;
                    _lupeImage.style.visibility = Visibility.Visible;
                    _nameTag.AddToClassList("NameTagLupe");
                    _nameTag.RemoveFromClassList("NameTagMisra");
                    _lupeImage.RemoveFromClassList("Inactive");
                    _misraImage.AddToClassList("Inactive");
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    _nameTag.style.visibility = Visibility.Visible;
                    _misraImage.style.visibility = Visibility.Visible;
                    _nameTag.AddToClassList("NameTagMisra");
                    _nameTag.RemoveFromClassList("NameTagLupe");
                    _misraImage.RemoveFromClassList("Inactive");
                    _lupeImage.AddToClassList("Inactive");
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
                    _lupeImage.style.visibility = Visibility.Hidden;
                    _lupeImage.AddToClassList("Inactive");
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    _misraImage.style.visibility = Visibility.Hidden;
                    _misraImage.AddToClassList("Inactive");
                }
            }
        }

        StartCoroutine(RollingTextRoutine(dialogue, 0.025f));
    }

    IEnumerator RollingTextRoutine(string fullText, float interval)
    {
        _isRolling = true;
        _continueTriangle.visible = false;
        _dialogueText.SetFullText(fullText); // << Set rolling text
        float buffer = 1f;

        for (int i = 0; i < _dialogueText.FullText.Length; i++)
        {
            _dialogueText.RollingTextStep();
            buffer -= interval;
            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(Mathf.Max(0, buffer) + 0.25f);
        if (_choicesActive)
        {
            _choiceParent.style.display = DisplayStyle.Flex;
        }
        _isRolling = false;
        _continueTriangle.visible = true;
    }
    #endregion

    #region ======== [[ STORY CHOICE ]] <PRIVATE_METHODS> ========================== >>>>
    /// <summary>
    /// Enables the choice buttons
    /// </summary>
    void PopulateChoices(List<Choice> choices)
    {
        _continueTriangle.style.visibility = Visibility.Hidden;

        DeselectAllButtons(false);

        int index = 0;
        foreach (Choice choice in choices)
        {
            _choiceButtons[index].text = choice.text;
            _choiceButtons[index].SetVisible(true);
            _choiceMap.Add(_choiceButtons[index]);
            index++;
        }

        _choicesActive = true;

        //choiceMap.SelectElement(choiceButtons[0]);
        _choiceMap.CurrentSelection.Select();
    }

    /// <summary>
    /// The function to change choice via input
    /// </summary>
    /// <param name="direction">The movement vector</param>
    void HandleSelectionInput(Vector2 direction)
    {
        direction.y = -direction.y;
        SelectableButton selected = _choiceMap.SelectElementInDirection(direction);
        if (selected != null)
        {
            _choiceMap.PreviousSelection.Deselect();
            selected.Select();
            //Debug.Log($"{PREFIX} >> Move: {move} - Selected: {selected.text}");
        }
        else
        {
            //Debug.LogError($"{PREFIX} >> Move: {move} - No Selected Move Target");
        }
    }

    /// <summary>
    /// The function to select choice via input
    /// </summary>
    void Select()
    {
        if (_isRolling)
        {
            StopAllCoroutines();
            _dialogueText.InstantCompleteText();
            _isRolling = false;
            _continueTriangle.visible = true;
        }
        else if (_choicesActive)
        {
            SelectChoice();
        }
        else
        {
            ContinueStory();
        }
    }

    /// <summary>
    /// Selects the choice at the given index
    /// </summary>
    void SelectChoice()
    {
        if (_choiceMap.CurrentSelection == null)
        {
            Debug.LogError($"{PREFIX} No choice selected.");
            return;
        }

        _continueTriangle.style.visibility = Visibility.Visible;

        _storyIterator.ChooseChoice(_choiceButtons.IndexOf(_choiceMap.CurrentSelection));
        _choicesActive = false;

        DeselectAllButtons(false);
        ContinueStory();
    }
    #endregion

    void DeselectAllButtons(bool visibility = true)
    {
        foreach (SelectableButton button in _choiceButtons)
        {
            button.Deselect();
            button.SetVisible(visibility);
        }
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
    /// Moves the cool dialogue triangle up and down
    /// </summary>
    void MoveTriangle()
    {
        _continueTriangle.ToggleInClassList("TriangleDown");
        _continueTriangle.RegisterCallback<TransitionEndEvent>(evt => _continueTriangle.ToggleInClassList("TriangleDown"));
        root.schedule.Execute(() => _continueTriangle.ToggleInClassList("TriangleDown")).StartingIn(100);
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
}
