using System.Collections;

using Darklight.UnityExt.UXML;

using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Utility;
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
public class GameUIController : UXML_UIDocumentObject
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
        selectableVectorField.Selectables.First().SetSelected();

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
            if (_pauseMenuContainer.visible)
            {
                SetVisibility(false);
                _pauseMenuContainer.visible = false;
                _menuFolder.visible = false;
                _settingsFolder.visible = false;
                _scenesFolder.visible = false;
                MTR_AudioManager.Instance.PlayMenuSelectEvent();
            }
        };

        SelectableButton settingsButton = ElementQuery<SelectableButton>(SETTINGS_BTN);
        settingsButton.OnClick += () =>
        {
            if (_pauseMenuContainer.visible)
            {
                _menuFolder.visible = false;
                _settingsFolder.visible = true;
                _scenesFolder.visible = true;
                MTR_AudioManager.Instance.PlayMenuSelectEvent();
            }
        };

        SelectableButton scenesButton = ElementQuery<SelectableButton>(SCENES_BTN);
        scenesButton.OnClick += () =>
        {
            if (_pauseMenuContainer.visible)
            {
                _menuFolder.visible = false;
                _settingsFolder.visible = false;
                _scenesFolder.visible = true;
                MTR_AudioManager.Instance.PlayMenuSelectEvent();
            }
        };



        SetVisibility(false);
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        if (_pauseMenuContainer.visible) // || _choicePanel.visible)
        {
            Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
            RotateChoiceSelection((int)directionInScreenSpace.y);

            // Select the next button in the direction
            SelectableButton oldButton = selectableVectorField.CurrentSelection;
            selectableVectorField.CurrentSelection.Deselect();
            SelectableButton newButton = selectableVectorField.GetElementInDirection(directionInScreenSpace);
            newButton?.SetSelected();

            if (oldButton != newButton) { MTR_AudioManager.Instance.PlayMenuHoverEvent(); }
        }
    }

    void RotateChoiceSelection(int direction)
    {
        if (_choiceButtons.Count == 0) return;

        selectedChoiceIndex += direction;
        if (selectedChoiceIndex < 0) selectedChoiceIndex = _choiceButtons.Count - 1;
        if (selectedChoiceIndex >= _choiceButtons.Count) selectedChoiceIndex = 0;

        Select(_choiceButtons[selectedChoiceIndex]);
    }

    void OnPrimaryInteractAction()
    {
        /*
        if (_choicePanel.visible)
        {
            _choiceButtons[selectedChoiceIndex].InvokeClickAction();
        }
        */

        selectableVectorField.CurrentSelection?.InvokeClickAction();
    }

    void OnMenuButtonAction()
    {
        if (_pauseMenuContainer.visible)
        {
            SetVisibility(false);
            _pauseMenuContainer.style.visibility = Visibility.Hidden;
            _menuFolder.visible = false;
            _settingsFolder.visible = false;
            _scenesFolder.visible = false;

        }
        else
        {
            SetVisibility(true);
            _pauseMenuContainer.style.visibility = Visibility.Visible;
            _menuFolder.visible = true;
        }
    }

    public void LoadChoices(List<Choice> choices)
    {
        _choiceBox = ElementQuery<GroupBox>("ChoiceBox");
        _choiceBox.Clear();
        _choiceButtons.Clear();
        _buttonHandlers.Clear();

        foreach (Choice choice in choices)
        {
            SelectableButton button = new SelectableButton();
            button.text = choice.text;

            Action handler = () => ConfirmChoice(choice);
            button.OnClick += handler;
            _buttonHandlers[button] = handler;

            _choiceBox.Add(button);
            _choiceButtons.Add(button);
        }

        // Select the first button
        Select(_choiceButtons[0]);

        _choicePanel.style.visibility = Visibility.Visible;
        _choicePanel.AddToClassList("visible");
        this.SetVisibility(true);
    }

    public void ConfirmChoice(Choice choice)
    {
        InkyStoryManager.Iterator.ChooseChoice(choice);
        MTR_AudioManager.Instance.PlayMenuSelectEvent();

        _choiceBox.Clear();
        _choicePanel.style.visibility = Visibility.Hidden;
        _choicePanel.RemoveFromClassList("visible");
    }

    void Select(SelectableButton newSelection)
    {
        if (newSelection == null || lockSelection) return;
        if (newSelection == selectedButton) return;

        // Transfer the selection
        previousButton = selectedButton;
        selectedButton = newSelection;

        // Set the selection classes
        previousButton?.Deselect();
        newSelection.SetSelected();

        if (previousButton != selectedButton) { MTR_AudioManager.Instance.PlayMenuHoverEvent(); }

        lockSelection = true;
        Invoke(nameof(UnlockSelection), 0.1f);

    }

    void UnlockSelection()
    {
        lockSelection = false;
    }

}
