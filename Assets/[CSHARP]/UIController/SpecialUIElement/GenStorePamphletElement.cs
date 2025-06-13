using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class GenStorePamphletElement : SpecialUIElement
{
    private List<VisualElement> _pages = new List<VisualElement>();

    [SerializeField, ShowOnly]
    private int _currentPageIndex = 0;

    public GenStorePamphletElement(GameUIController documentObject, string elementTag)
        : base(documentObject, elementTag)
    {
        InitializePages();
    }

    private void InitializePages()
    {
        _pages = new List<VisualElement>()
        {
            _documentObject.ElementQuery<VisualElement>("pamphlet-page-1"),
            _documentObject.ElementQuery<VisualElement>("pamphlet-page-2"),
            _documentObject.ElementQuery<VisualElement>("pamphlet-page-3"),
            _documentObject.ElementQuery<VisualElement>("pamphlet-page-4")
        };
    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
        MTR_AudioManager.Instance.PlayOneShotSFX(
            MTR_AudioManager.Instance.generalSFX.paperInteract
        );
        SetPamphletPage(0);
    }

    protected override void OnHide()
    {
        base.OnHide();
        HideAllPages();
    }

    protected override void RegisterInputEvents()
    {
        MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    protected override void UnregisterInputEvents()
    {
        MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    void OnMoveInputStartAction(Vector2 moveInput)
    {
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);
        if (direction.x > 0)
            OnRightDirectionInput();
        else if (direction.x < 0)
            OnLeftDirectionInput();
    }

    void OnPrimaryInteractAction()
    {
        MTR_AudioManager.Instance.PlayMenuSelectEvent();
        Display(false);
    }

    void SetPamphletPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= _pages.Count)
            return;

        HideAllPages();
        _pages[pageIndex].style.display = DisplayStyle.Flex;
        _currentPageIndex = pageIndex;
    }

    void HideAllPages()
    {
        foreach (VisualElement page in _pages)
            page.style.display = DisplayStyle.None;
    }

    void OnLeftDirectionInput()
    {
        _currentPageIndex--;
        SetPamphletPage(_currentPageIndex);
    }

    void OnRightDirectionInput()
    {
        _currentPageIndex++;
        SetPamphletPage(_currentPageIndex);
    }
}
