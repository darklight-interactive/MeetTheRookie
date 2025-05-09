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

    // VISUALS
    const string HOME_PAGE = "home-page";
    const string SETTINGS_PAGE = "settings-page";
    const string CONTROLS_PAGE = "controls-page";
    //const string SCENES_PAGE = "scenes-page";
    VisualElement _homePage;
    VisualElement _controlsPage; 
    VisualElement _settingsPage;
    //VisualElement _scenesPage;

    // BUTTONS
    const string RESUME_BTN = "resume-btn";
    //const string HOME_BTN = "home-btn";
    const string SETTINGS_BTN = "settings-btn";
    const string CONTROLS_BTN = "controls-btn";
    const string MAINMENU_BTN = "mainmenu-btn";
    const string RETURN_BTN_SETTINGS = "return-btn-settings";
    const string RETURN_BTN_CONTROLS = "return-btn-controls";
    SelectableButton _resumeButton;
    //SelectableButton _homeButton;
    SelectableButton _settingsButton;
    SelectableButton _controlsButton;
    SelectableButton _mainMenuButton;
    SelectableButton _returnButtonSettings;
    SelectableButton _returnButtonControls;
    //SelectableButton _controlsReturnButton;
    Dictionary<string, SelectableButton[]> buttonGroups;

    // SLIDERS
    const string MUSIC_BUS = "bus:/Music";
    const string SFX_BUS = "bus:/SFX";
    const string DIALOGUE_BUS = "bus:/Dialogue";


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
        // << VISUALS >>

        // Store references to the folders
        _homePage = ElementQuery<VisualElement>(HOME_PAGE);
        _controlsPage = ElementQuery<VisualElement>(CONTROLS_PAGE);
        _settingsPage = ElementQuery<VisualElement>(SETTINGS_PAGE);
        //_scenesPage = ElementQuery<VisualElement>(SCENES_PAGE);

        // Initilize visuals
        _homePage.style.display = DisplayStyle.None;
        _controlsPage.style.display = DisplayStyle.None;
        _settingsPage.style.display = DisplayStyle.None;
        // _scenesPage.style.display = DisplayStyle.None;

        

        // << BUTTONS >>

        // Initialize Buttons
        _resumeButton = ElementQuery<SelectableButton>(RESUME_BTN);
        _settingsButton = ElementQuery<SelectableButton>(SETTINGS_BTN);
        _controlsButton = ElementQuery<SelectableButton>(CONTROLS_BTN);
        _mainMenuButton = ElementQuery<SelectableButton>(MAINMENU_BTN);
        //_homeButton = ElementQuery<SelectableButton>(HOME_BTN);
        _returnButtonSettings = ElementQuery<SelectableButton>(RETURN_BTN_SETTINGS);
        _returnButtonControls = ElementQuery<SelectableButton>(RETURN_BTN_CONTROLS);

        buttonGroups = new Dictionary<string, SelectableButton[]>
        {
            { HOME_PAGE, new SelectableButton[]{ _resumeButton, _settingsButton, _controlsButton, _mainMenuButton } },
            { SETTINGS_PAGE, new SelectableButton[]{ _returnButtonSettings } },
            { CONTROLS_PAGE, new SelectableButton[]{ _returnButtonControls } }
        };

        // Load the selectable buttons
        selectableElements.Load(ElementQueryAll<SelectableButton>());
        selectableElements.RemoveRange(buttonGroups[SETTINGS_PAGE]);
        selectableElements.RemoveRange(buttonGroups[CONTROLS_PAGE]);



        // << BUTTON ACTIONS >>

        _resumeButton.OnClick += OnMenuButtonAction;

        
        _settingsButton.OnClick += () =>
        {
            selectableElements.AddRange(ElementQueryAll<SelectableSlider>());
            selectableElements.AddRange(buttonGroups[SETTINGS_PAGE]);
            selectableElements.RemoveRange(buttonGroups[HOME_PAGE]);

            _settingsPage.style.display = DisplayStyle.Flex;
            _homePage.style.display = DisplayStyle.None;

            //_homePage.visible = false;
            //_settingsPage.visible = true;
        };

        
        _controlsButton.OnClick += () =>
        {
            selectableElements.AddRange(buttonGroups[CONTROLS_PAGE]);
            selectableElements.RemoveRange(buttonGroups[HOME_PAGE]);

            _controlsPage.style.display = DisplayStyle.Flex;
            _homePage.style.display = DisplayStyle.None;

            //_homePage.visible = false;
            //_controlsPage.visible = true;
        };

        
        _mainMenuButton.OnClick += () =>
        {
            Debug.Log("Main menu button clicked");
        };

        //_homeButton.OnClick += () =>
        //{
        //    selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());

        //    _homePage.style.display = DisplayStyle.Flex;
        //    _controlsPage.style.display = DisplayStyle.None;
        //    _settingsPage.style.display = DisplayStyle.None;

        //    //_homePage.visible = true;
        //    //_controlsPage.visible = false;
        //    //_settingsPage.visible = false;
        //    //_scenesPage.visible = false;
        //};

        _returnButtonSettings.OnClick += () =>
        {
            selectableElements.AddRange(buttonGroups[HOME_PAGE]);
            selectableElements.RemoveRange(buttonGroups[SETTINGS_PAGE]);
            selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());

            _homePage.style.display = DisplayStyle.Flex;
            _settingsPage.style.display = DisplayStyle.None;
        };

        _returnButtonControls.OnClick += () =>
        {
            selectableElements.AddRange(buttonGroups[HOME_PAGE]);
            selectableElements.RemoveRange(buttonGroups[CONTROLS_PAGE]);

            _homePage.style.display = DisplayStyle.Flex;
            _controlsPage.style.display = DisplayStyle.None;

            //_homePage.visible = false;
            //_controlsPage.visible = true;
        };




        // << SLIDERS >>
        SelectableSlider musicSlider = ElementQuery<SelectableSlider>("music-slider");
        SelectableSlider sfxSlider = ElementQuery<SelectableSlider>("sfx-slider");
        SelectableSlider dialogueSlider = ElementQuery<SelectableSlider>("dialogue-slider");

        musicSlider.value = MTR_AudioManager.Instance.GetBus(MUSIC_BUS).Volume;
        sfxSlider.value = MTR_AudioManager.Instance.GetBus(SFX_BUS).Volume;
        dialogueSlider.value = MTR_AudioManager.Instance.GetBus(DIALOGUE_BUS).Volume;

        musicSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(MUSIC_BUS, musicSlider.value);
            //Debug.Log("MUSIC: " + musicSlider.value);
        };
        

        
        sfxSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(SFX_BUS, sfxSlider.value);
        };

        
        dialogueSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(DIALOGUE_BUS, dialogueSlider.value);
        };

        SetVisibility(false); // Is this necessary?

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
                if (dir.x > 0)
                {
                    slider.Increment();
                    MTR_AudioManager.Instance.PlayMenuSliderEvent();
                }
                else if (dir.x < 0)
                {
                    slider.Decrement();
                    MTR_AudioManager.Instance.PlayMenuSliderEvent();
                }
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
            //_homePage.style.visibility = Visibility.Hidden;
            //_controlsPage.style.visibility = Visibility.Hidden;
            //_settingsPage.style.visibility = Visibility.Hidden;
            //_scenesPage.style.visibility = Visibility.Hidden;

            selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());
            selectableElements.RemoveRange(buttonGroups[SETTINGS_PAGE]);
            selectableElements.RemoveRange(buttonGroups[CONTROLS_PAGE]);
            selectableElements.AddRange(buttonGroups[HOME_PAGE]);

            _homePage.style.display = DisplayStyle.None;
            _controlsPage.style.display = DisplayStyle.None;
            _settingsPage.style.display = DisplayStyle.None;
            //_homePage.visible = false;
            //_controlsPage.visible = false;
            //_settingsPage.visible = false;

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
            //_homePage.style.visibility = Visibility.Visible;
            //_controlsPage.style.visibility = Visibility.Visible;
            //_settingsPage.style.visibility = Visibility.Visible;
            //_scenesPage.style.visibility = Visibility.Visible;

            //selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());

            _homePage.style.display = DisplayStyle.Flex;
            //_homePage.visible = true;

            MTRSceneController.StateMachine.GoToState(MTRSceneState.PAUSE_MODE);

            // Listen to the input manager
            UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
            UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
            //UniversalInputManager.OnMenuButton += OnMenuButtonAction;
        }
    }



}
