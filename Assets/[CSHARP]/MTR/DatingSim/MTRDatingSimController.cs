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
using NaughtyAttributes;

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
    ControlledLabel _dialogueText;
    VisualElement _continueTriangle;
    VisualElement _lupe_nameTag;
    VisualElement _misra_nameTag;

    MTRCharacterControlElement _lupe_characterControl;
    MTRCharacterControlElement _misra_characterControl;


    VisualElement _choiceParent;
    List<SelectableButton> _choiceButtons = new List<SelectableButton>(4);

    private bool allowInkySFX;

    [HorizontalLine(color: EColor.Gray, order = 1)]
    [SerializeField, ShowOnly] bool _inputEnabled = false;
    [SerializeField, ShowOnly] string _knot = "";
    [SerializeField] SelectableVectorField<SelectableButton> _choiceMap = new SelectableVectorField<SelectableButton>();
    public bool inCar = false;
    [Tooltip("Next scene to load")] public SceneObject nextScene;
    [SerializeField][Tooltip("Place Dating Sim Emotes Asset Here Please")] private DatingSimEmotes emotes;

    // ================ [[ PROPERTIES ]] ================================ >>>>

    // ================ [[ UNITY METHODS ]] ============================ >>>>
    void Start()
    {
        MTRSceneController.StateMachine.OnStateChanged += OnSceneStateChanged;
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
        _knot = MTRSceneManager.Instance.ActiveSceneData.SceneKnot;
    }

    void InitializeDatingSim()
    {
        Debug.Log($"{PREFIX} >> Initialize");
        base.Initialize(preset);

        // << SET THE KNOT >>
        _knot = MTRSceneManager.Instance.GetActiveSceneScriptableData().SceneKnot;

        // << DISABLE INPUTS >>
        SetInputEnabled(false);
        ResetDatingSim();

        // << QUERY SELECTABLE BUTTONS >>
        IEnumerable<SelectableButton> temp = ElementQueryAll<SelectableButton>();
        _choiceButtons = temp.OrderBy(x => x.name).ToList();
        ResetChoiceMap();
        Debug.Log($"{PREFIX} >> Choice Buttons: {_choiceButtons.Count}");

        // << QUERY UXML ELEMENTS >>
        CreateTag(new List<string> { DIALOGUE_TAG, "text" }, out string dialogueTextTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "triangle" }, out string triangleTag);
        _dialogueText = ElementQuery<ControlledLabel>(dialogueTextTag);
        _continueTriangle = ElementQuery<VisualElement>(triangleTag);

        CreateTag(new List<string> { CHARACTER_TAG, "control", LUPE_TAG }, out string lupeCtrlTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag", LUPE_TAG }, out string lupeNameTag);
        _lupe_characterControl = ElementQuery<MTRCharacterControlElement>(lupeCtrlTag);
        _lupe_nameTag = ElementQuery<VisualElement>(lupeNameTag);


        CreateTag(new List<string> { CHARACTER_TAG, "control", MISRA_TAG }, out string misraCtrlTag);
        CreateTag(new List<string> { DIALOGUE_TAG, "nametag", MISRA_TAG }, out string misraNameTag);
        _misra_characterControl = ElementQuery<MTRCharacterControlElement>(misraCtrlTag);
        _misra_nameTag = ElementQuery<VisualElement>(misraNameTag);

        CreateTag(new List<string> { CHOICE_TAG, CONTAINER_TAG, "parent" }, out string choiceParentTag);
        _choiceParent = root.Q<VisualElement>(choiceParentTag);

        if (inCar) { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.Flex; }
        else { root.Q<VisualElement>("Dashboard").style.display = DisplayStyle.None; }

        if (!boundEmote)
        {
            // In Inky file function should be: EXTERNAL SetEmote(name, emote)
            MTRStoryManager.GlobalStory.BindExternalFunction("SetEmote", (object[] args) =>
            {
                return SetEmote((string)args[0], (string)args[1]);
            });
            boundEmote = true;
        }


        StartCoroutine(StartDatingSimRoutine());
    }

    void ResetDatingSim()
    {
        if (_dialogueText != null) _dialogueText.FullText = "";
        ResetChoiceMap();
    }

    IEnumerator StartDatingSimRoutine()
    {
        yield return new WaitForSeconds(2f);

        MTRStoryManager.OnNewDialogue += HandleStoryDialogue;
        MTRStoryManager.OnNewChoices += HandleStoryChoices;

        // Start story
        MTRStoryManager.GoToKnotOrStitch(_knot);
        MTRStoryManager.ContinueStory();

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
                EnableInput();
                break;
            case false:
                DisableInput();
                break;
        }
    }

    void EnableInput()
    {
        UniversalInputManager.OnMoveInputStarted += HandleSelectionInput;
        UniversalInputManager.OnPrimaryInteract += Select;
        _inputEnabled = true;
    }

    void DisableInput()
    {
        UniversalInputManager.OnMoveInputStarted -= HandleSelectionInput;
        UniversalInputManager.OnPrimaryInteract -= Select;
        _inputEnabled = false;
    }

    #region ======== [[ STORY DIALOGUE ]] <PRIVATE_METHODS> ========================== >>>>
    /// <summary>
    /// Update the dialogue 
    /// </summary>
    /// <param name="dialogue">The new dialogue</param>
    void HandleStoryDialogue(string dialogue)
    {
        string speaker = MTRStoryManager.CurrentSpeaker;
        HandleSpeaker(speaker);
        Debug.Log($"{PREFIX} >> Dialogue: {dialogue} - Speaker: {speaker}");

        MTRStoryManager.TryGetTags(out IEnumerable<string> tags);

        foreach (string tag in tags)
        {
            Debug.Log($"{PREFIX} >> Tag: {tag}");
            string[] splitTag = tag.ToLower().Split(":");
            if (splitTag[0].Trim() == "emote")
            {
                string[] content = splitTag[1].Split("|");
                SetEmote(content[0].Trim(), content[1].Trim());
            }
            else if (splitTag[0].Trim() == "hide")
            {
                if (splitTag[1].Trim() == "lupe")
                {
                    _lupe_characterControl.style.visibility = Visibility.Hidden;
                }
                else if (splitTag[1].Trim() == "misra")
                {
                    _misra_characterControl.style.visibility = Visibility.Hidden;
                }
            }
            else if (splitTag[0].Trim() == "sfx")
            {
                if (splitTag[1].Trim() == "on")
                {
                    allowInkySFX = true;
                    //Debug.Log("Inky SFX are allowed!");
                }
                else if (splitTag[1].Trim() == "off")
                {
                    allowInkySFX = false;
                    //Debug.Log("Inky SFX are banned!");
                }
            }
        }

        StartCoroutine(RollingTextRoutine(dialogue, 0.025f));
    }

    void HandleSpeaker(string speaker)
    {
        Debug.Log($"{PREFIX} >> Handle Speaker: {speaker}");
        switch (speaker)
        {
            case "Lupe":
                _lupe_nameTag.style.visibility = Visibility.Visible;
                _misra_nameTag.style.visibility = Visibility.Hidden;

                _lupe_characterControl.Active = true;
                _misra_characterControl.Active = false;
                break;
            case "Misra":
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
    void HandleStoryChoices(List<Choice> choices)
    {
        _continueTriangle.style.visibility = Visibility.Hidden;

        ResetChoiceMap();

        StartCoroutine(HandleStoryChoicesRoutine(choices));
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
        if (!_inputEnabled) return;

        direction.y = -direction.y;
        SelectableButton selected = _choiceMap.SelectElementInDirection(direction);
        if (selected != null)
        {
            if(_choiceMap.PreviousSelection != selected) { MTR_AudioManager.Instance.PlayMenuHoverEvent(); }
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
            MTRStoryManager.ContinueStory();
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
        SceneManager.LoadScene(nextScene);
    }







    /// <summary>
    /// Moves the cool dialogue triangle up and down
    /// </summary>
    void MoveTriangle()
    {
        _continueTriangle?.ToggleInClassList("TriangleDown");
        _continueTriangle?.RegisterCallback<TransitionEndEvent>(evt => _continueTriangle.ToggleInClassList("TriangleDown"));
        root.schedule.Execute(() => _continueTriangle?.ToggleInClassList("TriangleDown")).StartingIn(100);
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
        if (name == "lupe" )
        {
            _lupe_characterControl.style.visibility = Visibility.Visible;
            _lupe_characterControl.CharacterImage = emotes.currLupeEmote;

            if (allowInkySFX) { FMODExt_EventManager.PlayEventWithParametersByName(MTR_AudioManager.Instance.generalSFX.voiceLupe, (MTR_AudioManager.Instance.generalSFX.parameterNameLupe, emote)); }
        }
        else if (name == "misra")
        {
            _misra_characterControl.style.visibility = Visibility.Visible;
            _misra_characterControl.CharacterImage = emotes.currMisraEmote;

            if (allowInkySFX) { FMODExt_EventManager.PlayEventWithParametersByName(MTR_AudioManager.Instance.generalSFX.voiceMisra, (MTR_AudioManager.Instance.generalSFX.parameterNameMisra, emote)); }
        }

        return success;
    }
}
