using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : UXML_UIDocumentObject
{
    const string SELECTED_CLASS = "selected";
    const string LEFT_ARROW_BUTTON_TAG = "left-arrow-button";
    const string RIGHT_ARROW_BUTTON_TAG = "right-arrow-button";
    const string GEN_STORE_PAMPHLET_TAG = "gen-store-pamphlet";

    MTRStoryManager _storyManager;
    SelectableButton _previousSelectedButton;
    SelectableButton _currentSelectedButton;

    bool _displayGenStorePamphlet;

    [SerializeField]
    InkyStoryStitchData _genStorePamphletStitch;

    /// <summary>
    /// The current selectable elements in the pamphlet. This is updated based on the current state.
    /// </summary>
    SelectableVectorField<SelectableButton> _selectableVectorField =
        new SelectableVectorField<SelectableButton>();

    protected VisualElement _genStorePamphletElement;
    List<VisualElement> _genStorePamphletPages = new List<VisualElement>();
    int _currentPageIndex = 0;
    SelectableButton _leftArrowButton;
    SelectableButton _rightArrowButton;

    public void Awake()
    {
        Initialize(preset);
        DisplayGenStorePamphlet(false);

        MTRInteractionSystem.PlayerInteractor.OnInteractableAccepted += HandleInteractableAccepted;
        MTRStoryManager.OnRequestSpecialUI += HandleRequestSpecialUI;
    }

    void OnDestroy()
    {
        MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    void OnExitButtonClick()
    {
        DisplayGenStorePamphlet(false);
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        _storyManager = MTRStoryManager.Instance;
    }

    // Start is called before the first frame update
    void Start() { }

    void HandleInteractableAccepted(Interactable interactable)
    {
        if (interactable is MTRInteractable mtrInteractable)
        {
            if (mtrInteractable.Data.Key == _genStorePamphletStitch.Stitch)
            {
                DisplayGenStorePamphlet(true);
            }
        }
    }

    void HandleRequestSpecialUI(string ui)
    {
        switch (ui)
        {
            case "su_pamphlet":
                DisplayGenStorePamphlet(true);
                break;
            case "su_plaque":
                break;
            case "su_pinpad":
                break;
            default:
                break;
        }
    }

    public void DisplayGenStorePamphlet(bool display)
    {
        _displayGenStorePamphlet = display;

        _genStorePamphletElement = ElementQuery<VisualElement>(GEN_STORE_PAMPHLET_TAG);
        _genStorePamphletElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

        _genStorePamphletPages.Clear();
        _genStorePamphletPages = new List<VisualElement>()
        {
            ElementQuery<VisualElement>("pamphlet-page-1"),
            ElementQuery<VisualElement>("pamphlet-page-2"),
            ElementQuery<VisualElement>("pamphlet-page-3"),
            ElementQuery<VisualElement>("pamphlet-page-4")
        };

        _leftArrowButton = ElementQuery<SelectableButton>(LEFT_ARROW_BUTTON_TAG);
        _rightArrowButton = ElementQuery<SelectableButton>(RIGHT_ARROW_BUTTON_TAG);

        // Audio
        MTR_AudioManager.Instance.PlayOneShotSFX(
            MTR_AudioManager.Instance.generalSFX.paperInteract
        );

        if (display)
        {
            MTRSceneController.StateMachine?.GoToState(MTRSceneState.PAUSE_MODE);

            // Load the selectable elements
            _selectableVectorField.Clear();
            _selectableVectorField.Load(ElementQueryAll<SelectableButton>());

            MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
            MTRInputManager.OnMenuButton += () => DisplayGenStorePamphlet(false);

            _leftArrowButton.OnClick += OnLeftArrowButtonClick;
            _rightArrowButton.OnClick += OnRightArrowButtonClick;

            SetPamphletPage(0);
        }
        else
        {
            MTRSceneController.StateMachine?.GoToState(MTRSceneState.PLAY_MODE);
            _selectableVectorField.Clear();

            MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
            MTRInputManager.OnMenuButton -= () => DisplayGenStorePamphlet(false);

            _leftArrowButton.OnClick -= OnLeftArrowButtonClick;
            _rightArrowButton.OnClick -= OnRightArrowButtonClick;
        }
    }

    private void OnMoveInputStartAction(Vector2 moveInput)
    {
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);
        _currentSelectedButton = _selectableVectorField.SelectElementInDirection(direction);

        // If we have a new selection, update the previous and current selected buttons
        if (_currentSelectedButton != null && _currentSelectedButton != _previousSelectedButton)
        {
            // Remove the selected class from the previous selected button
            if (_previousSelectedButton != null)
                _previousSelectedButton.RemoveFromClassList(SELECTED_CLASS);

            // Add the selected class to the current selected button
            _currentSelectedButton.AddToClassList(SELECTED_CLASS);
            _previousSelectedButton = _currentSelectedButton;
        }
    }

    private void OnPrimaryInteractAction()
    {
        // Handle any primary interaction with the current page if needed
        MTR_AudioManager.Instance.PlayMenuSelectEvent();

        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.InvokeClickAction();
        }
    }

    void SetPamphletPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= _genStorePamphletPages.Count)
            return;

        HideAllPages();
        _genStorePamphletPages[pageIndex].style.display = DisplayStyle.Flex;
        _currentPageIndex = pageIndex;

        if (pageIndex == 0)
            _leftArrowButton.style.display = DisplayStyle.None;
        else if (pageIndex == _genStorePamphletPages.Count - 1)
            _rightArrowButton.style.display = DisplayStyle.None;
        else
        {
            _leftArrowButton.style.display = DisplayStyle.Flex;
            _rightArrowButton.style.display = DisplayStyle.Flex;
        }
    }

    void HideAllPages()
    {
        foreach (VisualElement page in _genStorePamphletPages)
            page.style.display = DisplayStyle.None;
    }

    [Button]
    public void ToggleGenStorePamphlet()
    {
        DisplayGenStorePamphlet(!_displayGenStorePamphlet);
    }

    [Button]
    public void OnLeftArrowButtonClick()
    {
        _currentPageIndex--;
        SetPamphletPage(_currentPageIndex);
    }

    [Button]
    public void OnRightArrowButtonClick()
    {
        _currentPageIndex++;
        SetPamphletPage(_currentPageIndex);
    }
}
