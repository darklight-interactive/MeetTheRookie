using System.Collections;

using Darklight.UnityExt.UXML;

using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Input;
using System.Linq;
using System.Collections.Generic;
using Ink.Runtime;
using Darklight.UnityExt.Inky;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This handles all of the Game UI elements like interactions and speech bubbles.
/// </summary>
public class PauseMenuController : UXML_UIDocumentObject
{
    const string PAUSE_MENU_CTN = "pausemenu-container";
    VisualElement _pauseMenuContainer;


    // << MENU ELEMENTS >>
    const string HOME_PAGE = "tab-container";
    const string CONTROLS_PAGE = "controls-page";
    const string SETTINGS_PAGE = "settings-page";
    //const string SCENES_PAGE = "scenes-page";
    VisualElement _homePage;
    VisualElement _controlsPage; 
    VisualElement _settingsPage;
    //VisualElement _scenesPage;


    const string RESUME_BTN = "resume-btn";
    const string HOME_BTN = "home-btn";
    const string CONTROLS_BTN = "controls-btn";
    const string SETTINGS_BTN = "settings-btn";
    //const string CONTROLS_RETURN_BTN = "return-btn";
    SelectableButton _resumeButton;
    SelectableButton _homeButton;
    SelectableButton _controlsButton;
    SelectableButton _settingsButton;
    //SelectableButton _controlsReturnButton;





    SelectableVectorField<VisualElement> selectableElements = new SelectableVectorField<VisualElement>();

    List<SelectableButton> _selectableButtons = new List<SelectableButton>();
    int selectedChoiceIndex = 0;

    public void Awake()
    {
        Initialize(preset);

        UniversalInputManager.OnMenuButton += OnMenuButtonAction;
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        // Store the pause menu container
        _pauseMenuContainer = ElementQuery<VisualElement>(PAUSE_MENU_CTN);
        _pauseMenuContainer.style.visibility = Visibility.Hidden;
    }

    public void Start()
    {
        // Load the selectable buttons
        selectableElements.Load(ElementQueryAll<SelectableButton>());

        
        // Store references to the folders
        _homePage = ElementQuery<VisualElement>(HOME_PAGE);
        _controlsPage = ElementQuery<VisualElement>(CONTROLS_PAGE);
        _settingsPage = ElementQuery<VisualElement>(SETTINGS_PAGE);
        //_scenesPage = ElementQuery<VisualElement>(SCENES_PAGE);

        // Initilize visuals
        _homePage.visible = false;
        _controlsPage.visible = false;
        _settingsPage.visible = false;
        //_scenesPage.visible = false;
        _homePage.style.display = DisplayStyle.Flex;
        _controlsPage.style.display = DisplayStyle.None;
        _settingsPage.style.display = DisplayStyle.None;
        // _scenesPage.style.display = DisplayStyle.None;

        // << PAUSE MENU ACTIONS >>
        _resumeButton = ElementQuery<SelectableButton>(RESUME_BTN);
        _resumeButton.OnClick += OnMenuButtonAction;

        _homeButton = ElementQuery<SelectableButton>(HOME_BTN);
        _homeButton.OnClick += () => // OnClick or OnSelect???
        {
            selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());

            _homePage.style.display = DisplayStyle.Flex;
            _controlsPage.style.display = DisplayStyle.None;
            _settingsPage.style.display = DisplayStyle.None;

            _homePage.visible = true;
            _controlsPage.visible = false;
            _settingsPage.visible = false;
            //_scenesPage.visible = false;
        };

        _controlsButton = ElementQuery<SelectableButton>(CONTROLS_BTN);
        _controlsButton.OnClick += () => // OnClick or OnSelect???
        {
            selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());

            _homePage.style.display = DisplayStyle.None;
            _controlsPage.style.display = DisplayStyle.Flex;
            _settingsPage.style.display = DisplayStyle.None;

            _homePage.visible = false;
            _controlsPage.visible = true;
            _settingsPage.visible = false;
            //_scenesPage.visible = false;
        };

        _settingsButton = ElementQuery<SelectableButton>(SETTINGS_BTN);
        _settingsButton.OnClick += () => // OnClick or OnSelect???
        {
            selectableElements.AddRange(ElementQueryAll<SelectableSlider>());

            _homePage.style.display = DisplayStyle.None;
            _controlsPage.style.display = DisplayStyle.None;
            _settingsPage.style.display = DisplayStyle.Flex;

            _homePage.visible = false;
            _controlsPage.visible = false;
            _settingsPage.visible = true;
            //_scenesPage.visible = false;
        };

        //_controlsReturnButton = ElementQuery<SelectableButton>(CONTROLS_RETURN_BTN);
        //_controlsReturnButton.OnSelect += () =>
        //{
        //    Debug.Log("CONTROLS RETURN BUTTON SELECTED");
        //};



        // SLIDERS
        SelectableSlider musicSlider = ElementQuery<SelectableSlider>("music-slider");
        musicSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Music", musicSlider.value);
        };
        Debug.Log("MUSIC: " + musicSlider.value);

        SelectableSlider sfxSlider = ElementQuery<SelectableSlider>("sfx-slider");
        sfxSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/SFX", sfxSlider.value);
        };

        SelectableSlider dialogueSlider = ElementQuery<SelectableSlider>("dialogue-slider");
        dialogueSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Dialogue", dialogueSlider.value);
        };


        musicSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Music").Volume;
        sfxSlider.value = MTR_AudioManager.Instance.GetBus("bus:/SFX").Volume;
        dialogueSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Dialogue").Volume;

        SetVisibility(false);
    }

    void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
        UniversalInputManager.OnMenuButton -= OnMenuButtonAction;
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        //RotateChoiceSelection((int)directionInScreenSpace.y);

        if (selectableElements.CurrentSelection is SelectableSlider slider)
        {
            if (dir.y == 0)
            {
                if (dir.x > 0) { slider.Increment(); }
                else if (dir.x < 0) { slider.Decrement(); }
                return;
            }
        }

        // Select the next button in the direction
        VisualElement oldButton = selectableElements.CurrentSelection;
        if (oldButton is SelectableSlider oldSlider)
        {
            oldSlider.Deselect();
        }
        else if (oldButton is SelectableButton button)
        {
            button.Deselect();
        }

        VisualElement newButton = selectableElements.SelectElementInDirection(directionInScreenSpace);
        if (newButton is SelectableSlider newSlider)
        {
            newSlider.Select();
        }
        else if (newButton is SelectableButton newSelectableButton)
        {
            newSelectableButton.Select();
        }

        if (directionInScreenSpace.y != 0.0 && oldButton != newButton) { MTR_AudioManager.Instance.PlayMenuHoverEvent(); }
    }

    void RotateChoiceSelection(int direction)
    {
        if (_selectableButtons.Count == 0) return;

        selectedChoiceIndex += direction;
        if (selectedChoiceIndex < 0) selectedChoiceIndex = _selectableButtons.Count - 1;
        if (selectedChoiceIndex >= _selectableButtons.Count) selectedChoiceIndex = 0;

    }

    void OnPrimaryInteractAction()
    {
        /*
        if (_choicePanel.visible)
        {
            _choiceButtons[selectedChoiceIndex].InvokeClickAction();
        }
        */

        if (_pauseMenuContainer.visible) { MTR_AudioManager.Instance.PlayMenuSelectEvent(); }
        if (selectableElements.CurrentSelection is SelectableButton button)
        {
            button.InvokeClickAction();
        }
    }

    void OnMenuButtonAction()
    {
        if (_pauseMenuContainer.visible)
        {
            Debug.Log("PAUSE MENU CONTAINER VISIBLE");

            SetVisibility(false);
            _pauseMenuContainer.style.visibility = Visibility.Hidden;
            _homePage.style.visibility = Visibility.Hidden;
            _controlsPage.style.visibility = Visibility.Hidden;
            _settingsPage.style.visibility = Visibility.Hidden;
            //_scenesPage.style.visibility = Visibility.Hidden;

            MTRSceneController.StateMachine.GoToState(MTRSceneState.PLAY_MODE);

            UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
            UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
            //UniversalInputManager.OnMenuButton -= OnMenuButtonAction;
        }
        else
        {
            if (MTRSceneController.StateMachine.CurrentState != MTRSceneState.PLAY_MODE) { return; }

            Debug.Log("PAUSE MENU CONTAINER NOT VISIBLE");
            //_resumeButton?.Select();

            SetVisibility(true);
            _pauseMenuContainer.style.visibility = Visibility.Visible;
            _homePage.style.visibility = Visibility.Visible;
            //_controlsPage.style.visibility = Visibility.Visible;
            //_settingsPage.style.visibility = Visibility.Visible;
            //_scenesPage.style.visibility = Visibility.Visible;

            MTRSceneController.StateMachine.GoToState(MTRSceneState.PAUSE_MODE);

            // Listen to the input manager
            UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
            UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
            //UniversalInputManager.OnMenuButton += OnMenuButtonAction;
        }
    }



}
