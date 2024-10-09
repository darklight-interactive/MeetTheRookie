using System.Collections;

using Darklight.UnityExt.UXML;

using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Input;
//using Darklight.UnityExt.Audio;
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
    const string PAUSEMENU_CTN = "PauseMenuContainer";
    VisualElement _pauseMenuContainer;

    // << MENU ELEMENTS >>
    const string MENU_FOLDER = "Menu";
    const string RESUME_BTN = "resume-btn";
    const string SETTINGS_BTN = "settings-btn";
    const string SCENES_BTN = "scenes-btn";
    const string MAINMENU_BTN = "mainmenu-btn";
    VisualElement _menuFolder;

    const string SETTINGS_FOLDER = "Settings";
    VisualElement _settingsFolder;

    const string SCENES_FOLDER = "Scenes";
    VisualElement _scenesFolder;



    SelectableVectorField<SelectableButton> selectableVectorField = new SelectableVectorField<SelectableButton>();

    List<SelectableButton> _choiceButtons = new List<SelectableButton>();
    int selectedChoiceIndex = 0;
    SelectableButton previousButton;
    SelectableButton selectedButton;

    private Dictionary<SelectableButton, Action> _buttonHandlers = new Dictionary<SelectableButton, Action>();

    VisualElement _header;
    VisualElement _body;
    VisualElement _footer;
    VisualElement _menuPanel;
    VisualElement _choicePanel;
    GroupBox _choiceBox;
    bool lockSelection = false;

    public void Awake()
    {
        Initialize(preset);
    }

    public void Start()
    {
        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
        UniversalInputManager.OnMenuButton += OnMenuButtonAction;

        // Load the selectable buttons
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        selectableVectorField.Selectables.First().Select();

        // Store the pause menu container
        _pauseMenuContainer = ElementQuery<VisualElement>(PAUSEMENU_CTN);
        _pauseMenuContainer.style.visibility = Visibility.Hidden;

        // Store references to the folders
        _menuFolder = ElementQuery<VisualElement>(MENU_FOLDER);
        _settingsFolder = ElementQuery<VisualElement>(SETTINGS_FOLDER);
        _scenesFolder = ElementQuery<VisualElement>(SCENES_FOLDER);

        // << PAUSE MENU ACTIONS >>
        SelectableButton resumeButton = ElementQuery<SelectableButton>(RESUME_BTN);
        resumeButton.OnClick += () =>
        {
            SetVisibility(false);
            _pauseMenuContainer.visible = false;
            _menuFolder.visible = false;
            _settingsFolder.visible = false;
            _scenesFolder.visible = false;
        };

        SelectableButton settingsButton = ElementQuery<SelectableButton>(SETTINGS_BTN);
        settingsButton.OnClick += () =>
        {
            _menuFolder.visible = false;
            _settingsFolder.visible = true;
            _scenesFolder.visible = true;
        };

        SelectableButton scenesButton = ElementQuery<SelectableButton>(SCENES_BTN);
        scenesButton.OnClick += () =>
        {
            _menuFolder.visible = false;
            _settingsFolder.visible = false;
            _scenesFolder.visible = true;
        };



        SetVisibility(false);
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        RotateChoiceSelection((int)directionInScreenSpace.y);

        // Select the next button in the direction
        SelectableButton oldButton = selectableVectorField.CurrentSelection;
        selectableVectorField.CurrentSelection.Deselect();
        SelectableButton newButton = selectableVectorField.SelectElementInDirection(directionInScreenSpace);
        newButton?.Select();

        if (directionInScreenSpace.y != 0.0 && oldButton != newButton) { MTR_AudioManager.Instance.PlayMenuHoverEvent(); }
    }

    void RotateChoiceSelection(int direction)
    {
        if (_choiceButtons.Count == 0) return;

        selectedChoiceIndex += direction;
        if (selectedChoiceIndex < 0) selectedChoiceIndex = _choiceButtons.Count - 1;
        if (selectedChoiceIndex >= _choiceButtons.Count) selectedChoiceIndex = 0;

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
        selectableVectorField.CurrentSelection?.InvokeClickAction();
    }

    void OnMenuButtonAction()
    {
        if (_pauseMenuContainer.visible)
        {
            SetVisibility(false);
            _pauseMenuContainer.style.visibility = Visibility.Hidden;
        }
        else
        {
            SetVisibility(true);
            _pauseMenuContainer.style.visibility = Visibility.Visible;
        }
    }



}
