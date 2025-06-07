using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : UXML_UIDocumentObject
{
    MTRStoryManager _storyManager;

    bool _displayGenStorePamphlet;

    [SerializeField]
    InkyStoryStitchData _genStorePamphletStitch;

    public List<VisualElement> Pages = new List<VisualElement>();
    public VisualElement currentselected;
    public int page;
    protected VisualElement _genStorePamphletElement =>
        ElementQuery<VisualElement>("gen-store-pamphlet");

    public void Awake()
    {
        Initialize(preset);
        DisplayGenStorePamphlet(false);

        MTRSceneController.Instance.OnSceneStateChanged += HandleSceneStateChanged;
        MTRInteractionSystem.PlayerInteractor.OnInteractableAccepted += HandleInteractableAccepted;
    }

    void OnDestroy()
    {
        MTRSceneController.Instance.OnSceneStateChanged -= HandleSceneStateChanged;
    }

    void HandleSceneStateChanged(MTRSceneState state)
    {
        if (state == MTRSceneState.PLAY_MODE)
            SetVisibility(true);
        else
            SetVisibility(false);
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        _storyManager = MTRStoryManager.Instance;
    }

    // Start is called before the first frame update
    void Start() { }

    void HandleInteractableAccepted(Interactable interactable)
    {
        if (interactable is MTRInteractable mtrInteractable)
        {
            if (mtrInteractable.Data.Key == _genStorePamphletStitch.Stitch)
            {
                DisplayGenStorePamphlet(true);
            }
        }
    }

    public void DisplayGenStorePamphlet(bool display)
    {
        _displayGenStorePamphlet = display;
        _genStorePamphletElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

        // Audio
        MTR_AudioManager.Instance.PlayOneShotSFX(
            MTR_AudioManager.Instance.generalSFX.paperInteract
        );

        if (display)
        {
            // << SETUP PAGES >>
            foreach (VisualElement page in _genStorePamphletElement.Children())
            {
                Pages.Add(page);
                page.AddToClassList("Unselected");
            }
            currentselected = Pages[page];
            currentselected.RemoveFromClassList("Unselected");

            MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        }
        else
        {
            MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        }
    }

    private void OnMoveInputStartAction(Vector2 moveInput)
    {
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);
        if (direction.x > 0)
        {
            if (page < 3)
            {
                currentselected.AddToClassList("Unselected");
                page += 1;
                currentselected = Pages[page];
                currentselected.RemoveFromClassList("Unselected");
            }
        }
        if (direction.x < 0)
        {
            if (page > 0)
            {
                currentselected.AddToClassList("Unselected");
                page -= 1;
                currentselected = Pages[page];
                currentselected.RemoveFromClassList("Unselected");
            }
        }
    }

    [Button]
    public void ToggleGenStorePamphlet()
    {
        DisplayGenStorePamphlet(!_displayGenStorePamphlet);
    }
}
