using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.FMODExt;
using Darklight.UnityExt.UXML;
using Ink.Runtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

#endif

public partial class MTRDatingSimController : UXML_UIDocumentObject
{
    #region ==== (( CONSTANTS )) ===================================== >>>>
    const string PREFIX = "< MTRDatingSimController >";
    const string DIALOGUE_TAG = "dialogue";
    const string CHARACTER_TAG = "character";
    const string CHOICE_TAG = "choice";
    const string CONTAINER_TAG = "container";
    const string IMAGE_TAG = "image";
    const string MISRA_TAG = "misra";
    const string LUPE_TAG = "lupe";

    const string ACTIVE_CLASS = "active";
    const string INACTIVE_CLASS = "inactive";
    #endregion

    static bool boundEmote = false;

    bool _choicesActive;
    bool _isRolling = false;

    Coroutine _rollingTextCoroutine;
    Coroutine _choicesCoroutine;

    ControlledLabel _dialogueLabel;
    VisualElement _continueTriangle;
    VisualElement _lupe_nameTag;
    VisualElement _misra_nameTag;

    MTRCharacterControlElement _lupe_characterControl;
    MTRCharacterControlElement _misra_characterControl;

    VisualElement _choiceParent;
    List<SelectableButton> _choiceButtons = new List<SelectableButton>(4);

    private bool allowInkySFX;

    [HorizontalLine(color: EColor.Gray, order = 1)]
    [SerializeField, ShowOnly]
    bool _inputEnabled = false;

    [SerializeField, ShowOnly]
    string _knot = "";

    [SerializeField]
    SelectableVectorField<SelectableButton> _choiceMap =
        new SelectableVectorField<SelectableButton>();
    public bool inCar = false;

    [SerializeField]
    [Tooltip("Place Dating Sim Emotes Asset Here Please")]
    private DatingSimEmotes emotes;

    // ================ [[ PROPERTIES ]] ================================ >>>>

    // ================ [[ UNITY METHODS ]] ============================ >>>>
    void Awake()
    {
        MTRSceneController.StateMachine.OnStateChanged += OnSceneStateChanged;

        MTRStoryManager.OnNewDialogue += HandleStoryDialogue;
        MTRStoryManager.OnNewChoices += HandleStoryChoices;
    }

    void OnDestroy()
    {
        SetInputEnabled(false); // Unbind input events

        MTRStoryManager.OnNewDialogue -= HandleStoryDialogue;
        MTRStoryManager.OnNewChoices -= HandleStoryChoices;

        MTRSceneController.StateMachine.OnStateChanged -= OnSceneStateChanged;
    }

    // ================ [[ INTERNAL METHODS ]] ========================== >>>>
    void OnSceneStateChanged(MTRSceneState newState)
    {
        switch (newState)
        {
            case MTRSceneState.INITIALIZE:
                ResetDatingSim();
                break;
            case MTRSceneState.ENTER:
                InitializeDatingSim();
                break;
        }
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        // << SET THE KNOT >>
        if (MTRSceneManager.ActiveSceneData != null)
            _knot = MTRSceneManager.ActiveSceneData.SceneKnot;

        // << QUERY UXML ELEMENTS >>
        CreateTag(new List<string> { DIALOGUE_TAG, "text" }, out string dialogueTextTag);
        _dialogueLabel = ElementQuery<ControlledLabel>(dialogueTextTag);
        _dialogueLabel.SetFullText("Hello Dating Sim!");
        _dialogueLabel.RollingTextPercentage = 1;
    }

    [Button]
    void InitializeDatingSim()
    {
        Debug.Log($"{PREFIX} >> Initialize");
        base.Initialize(preset);

        // << SET THE KNOT >>
        _knot = MTRSceneManager.ActiveSceneData.SceneKnot;

        // << DISABLE INPUTS >>
        SetInputEnabled(false);

        // << QUERY SELECTABLE BUTTONS >>
        IEnumerable<SelectableButton> temp = ElementQueryAll<SelectableButton>();
        _choiceButtons = temp.OrderBy(x => x.name).ToList();
        ResetChoiceMap();
        Debug.Log($"{PREFIX} >> Choice Buttons: {_choiceButtons.Count}");

        // << QUERY UXML ELEMENTS >>
        CreateTag(new List<string> { DIALOGUE_TAG, "text" }, out string dialogueTextTag);
        _dialogueLabel = ElementQuery<ControlledLabel>(dialogueTextTag);
        _dialogueLabel.FullText = "";

        CreateTag(new List<string> { DIALOGUE_TAG, "triangle" }, out string triangleTag);
        _continueTriangle = ElementQuery<VisualElement>(triangleTag);

        CreateTag(new List<string> { CHARACTER_TAG, "control", LUPE_TAG }, out string lupeCtrlTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag", LUPE_TAG }, out string lupeNameTag);
        _lupe_characterControl = ElementQuery<MTRCharacterControlElement>(lupeCtrlTag);
        if (_lupe_characterControl == null)
            Debug.LogError($"{PREFIX} >> Lupe Character Control not found");

        _lupe_nameTag = ElementQuery<VisualElement>(lupeNameTag);
        if (_lupe_nameTag == null)
            Debug.LogError($"{PREFIX} >> Lupe Name Tag not found");

        CreateTag(
            new List<string> { CHARACTER_TAG, "control", MISRA_TAG },
            out string misraCtrlTag
        );
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag", MISRA_TAG }, out string misraNameTag);
        _misra_characterControl = ElementQuery<MTRCharacterControlElement>(misraCtrlTag);
        if (_misra_characterControl == null)
            Debug.LogError($"{PREFIX} >> Misra Character Control not found");

        _misra_nameTag = ElementQuery<VisualElement>(misraNameTag);
        if (_misra_nameTag == null)
            Debug.LogError($"{PREFIX} >> Misra Name Tag not found");

        CreateTag(
            new List<string> { CHOICE_TAG, CONTAINER_TAG, "parent" },
            out string choiceParentTag
        );
        _choiceParent = root.Q<VisualElement>(choiceParentTag);

        if (inCar)
        {
            root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.Flex;
        }
        else
        {
            root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.None;
        }

        if (!boundEmote)
        {
            // In Inky file function should be: EXTERNAL SetEmote(name, emote)
            MTRStoryManager.GlobalStory.BindExternalFunction(
                "SetEmote",
                (object[] args) =>
                {
                    return SetEmote((string)args[0], (string)args[1]);
                }
            );
            boundEmote = true;
        }

        if (Application.isPlaying)
            StartCoroutine(StartDatingSimRoutine());
    }

    void ResetDatingSim()
    {
        if (_dialogueLabel != null)
            _dialogueLabel.FullText = "";
        ResetChoiceMap();
        Debug.Log($"{PREFIX} >> Reset Dating Sim");
    }

    IEnumerator StartDatingSimRoutine()
    {
        ResetDatingSim();

        yield return new WaitUntil(() => MTRStoryManager.IsInitialized);
        Debug.Log($"{PREFIX} >> Story Initialized: {MTRStoryManager.IsInitialized}");
        yield return new WaitUntil(() => MTRStoryManager.IsReady);
        Debug.Log($"{PREFIX} >> Story Ready: {MTRStoryManager.IsReady}");

        // Start story
        MTRStoryManager.GoToPath(_knot);
        MTRStoryManager.ContinueStory();
        Debug.Log($"{PREFIX} >> Initialize Dating Sim: {_knot}");

        MoveTriangle(); // Cool dialogue triangle movement

        // << ENABLE INPUTS >>
        SetInputEnabled(true);
        Debug.Log($"{PREFIX} >> Input Enabled: {_inputEnabled}");

        yield return null;
    }

    void CreateTag(List<string> tag_parts, out string outString)
    {
        outString = "";
        foreach (string part in tag_parts)
        {
            if (outString != "")
                outString += "-";
            outString += part;
        }
    }

    void SetInputEnabled(bool enabled)
    {
        switch (enabled)
        {
            case true:
                EnableInput();
                break;
            case false:
                DisableInput();
                break;
        }
        Debug.Log($"{PREFIX} >> Input Enabled: {enabled}");
    }

    void EnableInput()
    {
        MTRInputManager.OnMoveInputStarted += HandleSelectionInput;
        MTRInputManager.OnPrimaryInteract += HandlePrimaryInput;
        _inputEnabled = true;
    }

    void DisableInput()
    {
        MTRInputManager.OnMoveInputStarted -= HandleSelectionInput;
        MTRInputManager.OnPrimaryInteract -= HandlePrimaryInput;
        _inputEnabled = false;
    }

    /// <summary>
    /// The function to handle primary input
    /// </summary>
    void HandlePrimaryInput()
    {
        // If the rolling text is rolling, skip it
        if (_isRolling && _rollingTextCoroutine != null)
        {
            SkipRollingText();
            return;
        }

        // If the choices are active, and the choices coroutine is not running, select a choice
        if (_choicesActive)
        {
            SelectChoice();
            return;
        }
        else
        {
            Debug.LogWarning(
                $"{PREFIX} >> Primary Input: Cannot skip choice while coroutine is running"
            );
        }

        // If the story is not rolling, and the choices are not active, continue the story
        MTRStoryManager.ContinueStory();
    }

    #region ======== [[ STORY DIALOGUE ]] <PRIVATE_METHODS> ========================== >>>>
    /// <summary>
    /// Update the dialogue
    /// </summary>
    /// <param name="dialogue">The new dialogue</param>
    void HandleStoryDialogue(string dialogue)
    {
        // TODO: Convert tags to function calls

        MTRStoryManager.TryGetTags(out IEnumerable<string> tags);
        foreach (string tag in tags)
        {
            string[] splitTag = tag.ToLower().Split(":");

            string prefix = splitTag[0].Trim();
            string key = splitTag[1].Trim();

            Debug.Log($"{PREFIX} >> Tag: #{prefix}:{key}");

            switch (prefix)
            {
                case "name":
                    MTRSpeaker speaker = MTRStoryManager.GetSpeaker(key);
                    HandleSpeaker(speaker);
                    break;
                case "emote":
                    string[] emoteSplit = key.Split("|");
                    string characterName = emoteSplit[0].Trim();
                    string emoteName = emoteSplit[1].Trim();
                    SetEmote(characterName, emoteName);
                    break;
                case "hide":
                    if (key == "lupe")
                    {
                        _lupe_characterControl.style.visibility = Visibility.Hidden;
                    }
                    else if (key == "misra")
                    {
                        _misra_characterControl.style.visibility = Visibility.Hidden;
                    }
                    break;
                case "sfx":
                    if (key == "on")
                    {
                        allowInkySFX = true;
                    }
                    else if (key == "off")
                    {
                        allowInkySFX = false;
                    }
                    break;
            }
        }

        Debug.Log($"{PREFIX} >> Dialogue: {dialogue} - Speaker: {MTRStoryManager.CurrentSpeaker}");
        _rollingTextCoroutine = StartCoroutine(RollingTextRoutine(dialogue, 0.025f));
    }

    void HandleSpeaker(MTRSpeaker speaker)
    {
        Debug.Log($"{PREFIX} >> Handle Speaker: {speaker}");

        switch (speaker)
        {
            case MTRSpeaker.LUPE:
                _lupe_nameTag.style.visibility = Visibility.Visible;
                _misra_nameTag.style.visibility = Visibility.Hidden;

                _lupe_characterControl.Active = true;
                _misra_characterControl.Active = false;
                break;
            case MTRSpeaker.MISRA:
                _lupe_nameTag.style.visibility = Visibility.Hidden;
                _misra_nameTag.style.visibility = Visibility.Visible;

                _lupe_characterControl.Active = false;
                _misra_characterControl.Active = true;
                break;
            default:
                _lupe_nameTag.style.visibility = Visibility.Hidden;
                _misra_nameTag.style.visibility = Visibility.Hidden;

                _lupe_characterControl.Active = false;
                _misra_characterControl.Active = false;
                break;
        }
    }

    IEnumerator RollingTextRoutine(string fullText, float interval)
    {
        // Setup
        _isRolling = true;
        _continueTriangle.visible = false;
        _dialogueLabel.SetFullText(fullText); // << Set rolling text
        float buffer = 1f;

        // foreach character in the full text, step the rolling text
        for (int i = 0; i < _dialogueLabel.FullText.Length; i++)
        {
            _dialogueLabel.RollingTextStep();
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

    void SkipRollingText()
    {
        if (_rollingTextCoroutine != null)
        {
            StopCoroutine(_rollingTextCoroutine);
            _rollingTextCoroutine = null;
        }

        _dialogueLabel.InstantCompleteText();
        _isRolling = false;
    }
    #endregion

    #region ======== [[ STORY CHOICE ]] <PRIVATE_METHODS> ========================== >>>>
    /// <summary>
    /// Enables the choice buttons
    /// </summary>
    void HandleStoryChoices(List<Choice> choices)
    {
        _continueTriangle.style.visibility = Visibility.Hidden;

        ResetChoiceMap();

        _choicesCoroutine = StartCoroutine(HandleStoryChoicesRoutine(choices));
    }

    IEnumerator HandleStoryChoicesRoutine(List<Choice> choices)
    {
        yield return new WaitForSeconds(0.5f);
        int index = 0;
        foreach (Choice choice in choices)
        {
            _choiceButtons[index].text = choice.text;
            _choiceButtons[index].Enable();
            _choiceMap.Add(_choiceButtons[index]);
            index++;
            yield return new WaitForSeconds(0.25f);
        }
        _choicesActive = true;
        _choiceMap.CurrentSelection.Select();

        yield return new WaitForSeconds(0.25f);
        ResetCharacterControls();

        yield return null;
    }

    /// <summary>
    /// The function to change choice via input
    /// </summary>
    /// <param name="direction">The movement vector</param>
    void HandleSelectionInput(Vector2 direction)
    {
        if (!_inputEnabled)
            return;

        direction.y = -direction.y;
        SelectableButton selected = _choiceMap.SelectElementInDirection(direction);
        if (selected != null)
        {
            if (_choiceMap.PreviousSelection != selected)
            {
                MTR_AudioManager.Instance.PlayMenuHoverEvent();
            }
            _choiceMap.PreviousSelection.Deselect();
            selected.Select();

            DisableInput();
            Invoke(nameof(EnableInput), 0.25f);

            Debug.Log($"{PREFIX} >> Move: {direction} - Selected: {selected.text}");
        }
        else
        {
            //Debug.LogError($"{PREFIX} >> Move: {move} - No Selected Move Target");
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

        MTRStoryManager.ChooseChoice(_choiceButtons.IndexOf(_choiceMap.CurrentSelection));
        _choicesActive = false;

        ResetChoiceMap();
        MTRStoryManager.ContinueStory();
    }
    #endregion

    void ResetChoiceMap()
    {
        foreach (SelectableButton button in _choiceButtons)
        {
            button.Deselect();
            button.Disable();
        }

        if (_choiceMap != null && _choiceMap.Selectables.Count > 0)
            _choiceMap.Clear();
    }

    void ResetCharacterControls()
    {
        _lupe_characterControl.Active = false;
        _misra_characterControl.Active = false;
    }

    /// <summary>
    /// Ends the story. Transition to next scene from here.
    /// </summary>
    void EndStory()
    {
        HandleStoryDialogue("END OF STORY");
        Debug.Log("END OF STORY");
    }

    /// <summary>
    /// Moves the cool dialogue triangle up and down
    /// </summary>
    void MoveTriangle()
    {
        _continueTriangle?.ToggleInClassList("TriangleDown");
        _continueTriangle?.RegisterCallback<TransitionEndEvent>(evt =>
            _continueTriangle.ToggleInClassList("TriangleDown")
        );
        root.schedule.Execute(() => _continueTriangle?.ToggleInClassList("TriangleDown"))
            .StartingIn(100);
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
            _lupe_characterControl.style.visibility = Visibility.Visible;
            _lupe_characterControl.CharacterImage = emotes.currLupeEmote;

            if (allowInkySFX)
            {
                FMODExt_EventManager.PlayEventWithParametersByName(
                    MTR_AudioManager.Instance.generalSFX.voiceLupe,
                    (MTR_AudioManager.Instance.generalSFX.parameterNameLupe, emote)
                );
            }
        }
        else if (name == "misra")
        {
            _misra_characterControl.style.visibility = Visibility.Visible;
            _misra_characterControl.CharacterImage = emotes.currMisraEmote;

            if (allowInkySFX)
            {
                FMODExt_EventManager.PlayEventWithParametersByName(
                    MTR_AudioManager.Instance.generalSFX.voiceMisra,
                    (MTR_AudioManager.Instance.generalSFX.parameterNameMisra, emote)
                );
            }
        }

        return success;
    }
}
