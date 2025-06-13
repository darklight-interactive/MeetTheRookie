using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class WineryPinPadElement : SpecialUIElement
{
    VisualElement _groupbase;
    Label _numbers;
    VisualElement _currentselected;
    List<VisualElement> _buttons = new List<VisualElement>();
    VisualElement _lFlasher;
    VisualElement _rFlasher;
    private int selector = 0;

    [SerializeField, ShowOnly]
    string _correctcode = "100722";

    [SerializeField, ShowOnly]
    [Tooltip("Current number of input values")]
    int _numInputValues = 0;

    [SerializeField, ShowOnly]
    bool _isPinPadCorrect;

    [SerializeField]
    InkyStoryStitchData _correctCodeStitchData;

    public InkyStoryStitchData CorrectCodeStitchData => _correctCodeStitchData;

    public WineryPinPadElement(
        GameUIController documentObject,
        string elementTag,
        InkyStoryStitchData correctCodeStitchData
    )
        : base(documentObject, elementTag)
    {
        _correctCodeStitchData = correctCodeStitchData;

        _groupbase = _documentObject.ElementQuery<VisualElement>("Base");
        _numbers = _documentObject.ElementQuery<VisualElement>("Numbers").Q<Label>("Numbers");
        _lFlasher = _documentObject.ElementQuery<VisualElement>("LFlasher");
        _rFlasher = _documentObject.ElementQuery<VisualElement>("RFlasher");
        foreach (VisualElement row in _groupbase.Children())
        {
            foreach (VisualElement child in row.Children())
            {
                _buttons.Add(child);
            }
        }
        _buttons.Remove(_numbers);
        _currentselected = _buttons[selector];
        _currentselected.RemoveFromClassList("UnHovered");
        _currentselected.AddToClassList("Hovered");
        _groupbase.style.scale = new StyleScale(new Scale(new Vector2(1.4f, 1.4f)));
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

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y);
        if (directionInScreenSpace.x > 0)
        {
            if (selector < 11)
            {
                UnHover();
                selector += 1;
                _currentselected = _buttons[selector];
                Hover();
            }
        }
        if (directionInScreenSpace.y > 0)
        {
            if (selector < 9)
            {
                UnHover();
                selector += 3;
                _currentselected = _buttons[selector];
                Hover();
            }
        }
        if (directionInScreenSpace.x < 0)
        {
            if (selector > 0)
            {
                UnHover();
                selector -= 1;
                _currentselected = _buttons[selector];
                Hover();
            }
        }
        if (directionInScreenSpace.y < 0)
        {
            if (selector > 2)
            {
                UnHover();
                selector -= 3;
                _currentselected = _buttons[selector];
                Hover();
            }
        }
    }

    // Update is called once per frame
    void Update() { }

    void OnPrimaryInteractAction()
    {
        _documentObject.StartCoroutine(Select());
    }

    IEnumerator Select()
    {
        _currentselected.RemoveFromClassList("Hovered");
        _currentselected.AddToClassList("Selected");
        {
            if (_numInputValues < 6)
            {
                _numInputValues += 1;
                _numbers.text += _currentselected.name;
                MTR_AudioManager.Instance.PlayOneShotSFX(
                    MTR_AudioManager.Instance.generalSFX.pinpadNumber
                );
            }
        }
        if (_currentselected.name == "Star")
        {
            _numInputValues = 0;
            _numbers.text = "";
        }
        if (_currentselected.name == "Pound")
        {
            if (_numbers.text != _correctcode)
            {
                _documentObject.StartCoroutine(Incorrect());
            }
            if (_numbers.text == _correctcode)
            {
                _documentObject.StartCoroutine(Correct());
            }
        }
        yield return new WaitForSecondsRealtime(0.3f);
        _currentselected.RemoveFromClassList("Selected");
        _currentselected.AddToClassList("Hovered");
    }

    IEnumerator Correct()
    {
        MTR_AudioManager.Instance.PlayOneShotSFX(
            MTR_AudioManager.Instance.generalSFX.pinpadSuccess
        );
        yield return new WaitForSecondsRealtime(0.6f);
        _lFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
        _isPinPadCorrect = true;
        OnPinPadCorrectAction();
    }

    IEnumerator Incorrect()
    {
        MTR_AudioManager.Instance.PlayOneShotSFX(MTR_AudioManager.Instance.generalSFX.pinpadFail);
        _rFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
        yield return new WaitForSecondsRealtime(0.2f);
        _rFlasher.style.unityBackgroundImageTintColor = new StyleColor(
            new Color(.4156f, .4156f, .4156f, 1)
        );
        yield return new WaitForSecondsRealtime(0.2f);
        _rFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
        yield return new WaitForSecondsRealtime(0.2f);
        _rFlasher.style.unityBackgroundImageTintColor = new StyleColor(
            new Color(.4156f, .4156f, .4156f, 1)
        );
    }

    void OnPinPadCorrectAction()
    {
        Display(false);

        // << FORCED INTERACTION WITH CORRECT CODE STITCH >>
        MTRInteractionSystem.TryGetInteractableByStitch(
            _correctCodeStitchData.Stitch,
            out var interactable
        );
        Debug.Log($"OnPinPadCorrectAction {_correctCodeStitchData.Stitch} : {interactable}");
        if (interactable != null)
            MTRInteractionSystem.PlayerInteractor.InteractWith(interactable, true);

        MTR_Misra_Controller misraController =
            UnityEngine.Object.FindFirstObjectByType<MTR_Misra_Controller>();
        if (misraController != null)
        {
            misraController.enabled = true;
            misraController.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void UnHover()
    {
        _currentselected.RemoveFromClassList("Selected");
        _currentselected.RemoveFromClassList("Hovered");
        _currentselected.AddToClassList("UnHovered");
    }

    void Hover()
    {
        _currentselected.RemoveFromClassList("UnHovered");
        _currentselected.AddToClassList("Hovered");
    }
}
