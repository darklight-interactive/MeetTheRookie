using System;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Base class for all special UI elements that can be displayed in the game
/// </summary>
[Serializable]
public abstract class SpecialUIElement
{
    protected const string SELECTED_CLASS = "selected";

    protected VisualElement _rootElement;

    [SerializeField, ShowOnly]
    protected bool _isDisplayed;

    [SerializeField, ShowOnly]
    protected string _elementTag;
    protected UXML_UIDocumentObject _documentObject;

    public bool IsDisplayed => _isDisplayed;

    public SpecialUIElement(
        UXML_UIDocumentObject documentObject,
        string elementTag,
        bool display = false
    )
    {
        _documentObject = documentObject;
        _elementTag = elementTag;
        _rootElement = documentObject.ElementQuery<VisualElement>(elementTag);
        Display(display);
    }

    public virtual void Display(bool display)
    {
        _isDisplayed = display;
        _rootElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

        if (display)
        {
            OnDisplay();
        }
        else
        {
            OnHide();
        }
    }

    protected virtual void OnDisplay()
    {
        MTRSceneController.StateMachine?.GoToState(MTRSceneState.PAUSE_MODE);
        RegisterInputEvents();
    }

    protected virtual void OnHide()
    {
        MTRSceneController.StateMachine?.GoToState(MTRSceneState.PLAY_MODE);
        UnregisterInputEvents();
    }

    protected abstract void RegisterInputEvents();
    protected abstract void UnregisterInputEvents();
}

[Serializable]
public class BaseSpecialUIElement : SpecialUIElement
{
    public BaseSpecialUIElement(
        UXML_UIDocumentObject documentObject,
        string elementTag,
        bool display = false
    )
        : base(documentObject, elementTag, display) { }

    protected override void RegisterInputEvents()
    {
        MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    protected override void UnregisterInputEvents()
    {
        MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    protected virtual void OnPrimaryInteractAction()
    {
        Display(false);
    }
}
