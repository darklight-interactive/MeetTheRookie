using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : UXML_UIDocumentObject
{
    const string INPUT_UI_CONTAINER_TAG = "input-ui-container";
    const string GEN_STORE_PAMPHLET_TAG = "gen-store-pamphlet";
    const string PIN_PAD_TAG = "winery-pinpad";
    const string PLAQUE_TAG = "memorial-plaque";
    const string HANDWRITTEN_NOTE_TAG = "handwritten-note";
    const string BLUEPRINT_TAG = "winery-blueprint";

    VisualElement _inputUIContainer;

    [SerializeField]
    GenStorePamphletElement _genStorePamphletElement;

    [SerializeField]
    WineryPinPadElement _wineryPinPadElement;

    [SerializeField]
    BaseSpecialUIElement _memorialPlaqueElement;

    [SerializeField]
    BaseSpecialUIElement _handwrittenNoteElement;

    [SerializeField]
    BaseSpecialUIElement _blueprintElement;

    void Awake()
    {
        MTRStoryManager.OnRequestSpecialUI += HandleRequestSpecialUI;
        MTRSceneController.Instance.OnSceneStateChanged += OnSceneStateChanged;
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);
        _inputUIContainer = ElementQuery<VisualElement>(INPUT_UI_CONTAINER_TAG);
        _genStorePamphletElement = new GenStorePamphletElement(this, GEN_STORE_PAMPHLET_TAG);
        _memorialPlaqueElement = new BaseSpecialUIElement(this, PLAQUE_TAG);
        _handwrittenNoteElement = new BaseSpecialUIElement(this, HANDWRITTEN_NOTE_TAG);
        _blueprintElement = new BaseSpecialUIElement(this, BLUEPRINT_TAG);

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

    void OnSceneStateChanged(MTRSceneState state)
    {
        _inputUIContainer = ElementQuery<VisualElement>(INPUT_UI_CONTAINER_TAG);

        if (state == MTRSceneState.PAUSE_MODE || state == MTRSceneState.SYNTHESIS_MODE)
        {
            _inputUIContainer.style.display = DisplayStyle.None;
        }
        else
        {
            _inputUIContainer.style.display = DisplayStyle.Flex;
        }
    }

    void OnDestroy()
    {
        MTRStoryManager.OnRequestSpecialUI -= HandleRequestSpecialUI;
        MTRSceneController.Instance.OnSceneStateChanged -= OnSceneStateChanged;
    }

    // Receiving requests from the story manager
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
            case "su_blueprint":
                _blueprintElement.Display(true);
                break;
            default:
                break;
        }
    }

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
