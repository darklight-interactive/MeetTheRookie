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
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    SelectableVectorField<SelectableButton> choiceButtonField = new SelectableVectorField<SelectableButton>();
    private Dictionary<SelectableButton, Action> buttonHandlers = new Dictionary<SelectableButton, Action>();

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

        _choicePanel = ElementQuery<VisualElement>("ChoicePanel");
        _choicePanel.style.visibility = Visibility.Hidden;


        SetVisibility(false);
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        SelectableButton buttonInDirection = choiceButtonField.GetElementInDirection(directionInScreenSpace);
        Select(buttonInDirection);
    }

    void OnPrimaryInteractAction()
    {
        //selectableVectorField.CurrentSelection?.Clic
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
        choiceButtonField.Clear();
        buttonHandlers.Clear();

        foreach (Choice choice in choices)
        {
            SelectableButton button = new SelectableButton();
            //button.text = choice.text;
            Action handler = () => SelectChoice(choice);
            button.OnClick += handler;
            _choiceBox.Add(button);
            buttonHandlers[button] = handler;
        }

        // Load the Selectable Elements
        choiceButtonField.Load(ElementQueryAll<SelectableButton>());
        choiceButtonField.Selectables.First().SetSelected();
    }

    public void SelectChoice(Choice choice)
    {
        InkyStoryManager.Iterator.ChooseChoice(choice);
        //FMODEventManager.PlayOneShot(MTR_AudioManager.Instance.menuSelectEventReference);

        // Remove OnClick event handlers for each button
        foreach (SelectableButton button in choiceButtonField.Selectables)
        {
            if (buttonHandlers.TryGetValue(button, out var handler))
            {
                button.OnClick -= handler;
            }
        }

        _choiceBox.Clear();
    }





    void Select(SelectableButton selectedButton)
    {
        if (selectedButton == null || lockSelection) return;

        SelectableButton previousButton = choiceButtonField.PreviousSelection;
        if (selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.SetSelected();
            lockSelection = true;
            //FMODEventManager.PlayOneShot(MTR_AudioManager.Instance.menuHoverEventReference);
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void UnlockSelection()
    {
        lockSelection = false;
    }





}
