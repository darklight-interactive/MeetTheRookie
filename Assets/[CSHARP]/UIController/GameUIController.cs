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
public class GameUIController : UXML_UIDocumentObject
{
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

        _body = ElementQuery<VisualElement>("body");
        _header = ElementQuery<VisualElement>("header");
        _footer = ElementQuery<VisualElement>("footer");

        _menuPanel = ElementQuery<VisualElement>("MenuPanel");
        _menuPanel.style.visibility = Visibility.Hidden;

        _choicePanel = ElementQuery<VisualElement>("ChoicePanel");
        _choicePanel.style.visibility = Visibility.Hidden;


        SetVisibility(false);
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        RotateChoiceSelection((int)directionInScreenSpace.y);

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
        if (_choicePanel.visible)
        {
            _choiceButtons[selectedChoiceIndex].InvokeClickAction();
        }
    }

    void OnMenuButtonAction()
    {
        if (_menuPanel.visible)
        {
            SetVisibility(false);
            _menuPanel.visible = false;
        }
        else
        {
            SetVisibility(true);
            _menuPanel.visible = true;
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
        MTR_AudioManager.PlayOneShot(MTR_AudioManager.Instance.menuSelectEventReference);

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

        lockSelection = true;
        MTR_AudioManager.PlayOneShot(MTR_AudioManager.Instance.menuHoverEventReference);
        Invoke(nameof(UnlockSelection), 0.1f);

    }

    void UnlockSelection()
    {
        lockSelection = false;
    }





}
