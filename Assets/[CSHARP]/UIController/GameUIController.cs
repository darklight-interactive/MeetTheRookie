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

    MTRStoryManager _storyManager;
    SelectableButton _previousSelectedButton;
    SelectableButton _currentSelectedButton;

    bool _displayGenStorePamphlet;

    [SerializeField]
    InkyStoryStitchData _genStorePamphletStitch;

    public List<VisualElement> Pages = new List<VisualElement>();
    public int page;

    /// <summary>
    /// The current selectable elements in the pamphlet. This is updated based on the current state.
    /// </summary>
    SelectableVectorField<SelectableButton> _selectableVectorField =
        new SelectableVectorField<SelectableButton>();

    protected VisualElement genStorePamphletElement =>
        ElementQuery<VisualElement>("gen-store-pamphlet");

    public void Awake()
    {
        Initialize(preset);
        DisplayGenStorePamphlet(false);

        MTRInteractionSystem.PlayerInteractor.OnInteractableAccepted += HandleInteractableAccepted;
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

    void OnLeftArrowButtonClick()
    {
        Debug.Log("Left Arrow Button Clicked");
    }

    void OnRightArrowButtonClick()
    {
        Debug.Log("Right Arrow Button Clicked");
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

    public void DisplayGenStorePamphlet(bool display)
    {
        _displayGenStorePamphlet = display;
        genStorePamphletElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

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
        }
        else
        {
            MTRSceneController.StateMachine?.GoToState(MTRSceneState.PLAY_MODE);
            _selectableVectorField.Clear();

            MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
            MTRInputManager.OnMenuButton -= () => DisplayGenStorePamphlet(false);
        }
    }

    private void OnMoveInputStartAction(Vector2 moveInput)
    {
        if (!_displayGenStorePamphlet)
            return;

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
        if (!_displayGenStorePamphlet)
            return;

        // Handle any primary interaction with the current page if needed
        MTR_AudioManager.Instance.PlayMenuSelectEvent();
    }

    [Button]
    public void ToggleGenStorePamphlet()
    {
        DisplayGenStorePamphlet(!_displayGenStorePamphlet);
    }
}
