using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : UXML_UIDocumentObject
{
    const string GEN_STORE_PAMPHLET_TAG = "gen-store-pamphlet";
    const string PIN_PAD_TAG = "winery-pinpad";
    const string PLAQUE_TAG = "memorial-plaque";
    const string HANDWRITTEN_NOTE_TAG = "handwritten-note";

    [SerializeField]
    GenStorePamphletElement _genStorePamphletElement;

    [SerializeField]
    WineryPinPadElement _wineryPinPadElement;

    [SerializeField]
    BaseSpecialUIElement _memorialPlaqueElement;

    [SerializeField]
    BaseSpecialUIElement _handwrittenNoteElement;

    void Awake()
    {
        MTRStoryManager.OnRequestSpecialUI += HandleRequestSpecialUI;
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);
        _genStorePamphletElement = new GenStorePamphletElement(this, GEN_STORE_PAMPHLET_TAG);
        _memorialPlaqueElement = new BaseSpecialUIElement(this, PLAQUE_TAG);
        _handwrittenNoteElement = new BaseSpecialUIElement(this, HANDWRITTEN_NOTE_TAG);

        // Initialize the pin pad element with the correct code stitch data
        if (_wineryPinPadElement != null)
            _wineryPinPadElement = new WineryPinPadElement(
                this,
                PIN_PAD_TAG,
                _wineryPinPadElement.CorrectCodeStitchData
            );
        else
        {
            _wineryPinPadElement = new WineryPinPadElement(this, PIN_PAD_TAG, null);
        }
    }

    void OnDestroy()
    {
        MTRStoryManager.OnRequestSpecialUI -= HandleRequestSpecialUI;
    }

    void HandleRequestSpecialUI(string ui_request)
    {
        Initialize();

        switch (ui_request)
        {
            case "su_pamphlet":
                _genStorePamphletElement.Display(true);
                break;
            case "su_note":
                _handwrittenNoteElement.Display(true);
                break;
            case "su_plaque":
                _memorialPlaqueElement.Display(true);
                break;
            case "su_pinpad":
                _wineryPinPadElement.Display(true);
                break;
            default:
                break;
        }
    }

    /*
    void DisplayPinPad(bool display)
    {
        _pinPadIsDisplayed = display;

        _pinPadElement = ElementQuery<VisualElement>(PIN_PAD_TAG);
        _pinPadElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        _pinPadScript.EnablePinPad(display);

        if (display)
        {
            MTRSceneController.StateMachine?.GoToState(MTRSceneState.PAUSE_MODE);

            MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
            MTRInputManager.OnMenuButton += OnMenuButtonAction;

            _pinPadScript.OnPinPadCorrect += OnPinPadCorrectAction;
        }
        else
        {
            MTRSceneController.StateMachine?.GoToState(MTRSceneState.PLAY_MODE);

            MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
            MTRInputManager.OnMenuButton -= OnMenuButtonAction;

            _pinPadScript.OnPinPadCorrect -= OnPinPadCorrectAction;
        }
    }
    */


    [Button]
    public void TogglePamphlet()
    {
        _genStorePamphletElement.Display(!_genStorePamphletElement.IsDisplayed);
    }

    [Button]
    public void TogglePinPad()
    {
        _wineryPinPadElement.Display(!_wineryPinPadElement.IsDisplayed);
    }

    [Button]
    public void ToggleMemorialPlaque()
    {
        _memorialPlaqueElement.Display(!_memorialPlaqueElement.IsDisplayed);
    }

    [Button]
    public void ToggleHandwrittenNote()
    {
        _handwrittenNoteElement.Display(!_handwrittenNoteElement.IsDisplayed);
    }
}
