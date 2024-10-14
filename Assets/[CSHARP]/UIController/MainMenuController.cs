using System.Linq;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : UXML_UIDocumentObject
{
    MTRSceneManager sceneManager;
    SelectableButton playButton;
    SelectableButton optionsButton;
    SelectableButton quitButton;
    SelectableVectorField<SelectableButton> selectableVectorField = new SelectableVectorField<SelectableButton>();
    bool lockSelection = false;

    public void Awake()
    {
        sceneManager = MTRSceneManager.Instance as MTRSceneManager;
        Initialize(preset);
    }

    void Start()
    {
        // Store the local references to the buttons
        playButton = ElementQuery<SelectableButton>("play-button");
        optionsButton = ElementQuery<SelectableButton>("options-button");
        quitButton = ElementQuery<SelectableButton>("quit-button");

        // Assign the events
        playButton.OnClick += PlayButtonAction;
        optionsButton.OnClick += () => Debug.Log("Options Button Clicked");
        quitButton.OnClick += Quit;

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        selectableVectorField.Selectables.First().Select();

        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        SelectableButton button = selectableVectorField.SelectElementInDirection(directionInScreenSpace);
        Select(button);
        Debug.Log($"MainMenuController: OnMoveInputStartAction({dir}) -> {button.name}");
    }

    void Select(SelectableButton selectedButton)
    {
        if (selectedButton == null || lockSelection) return;

        SelectableButton previousButton = selectableVectorField.PreviousSelection;
        if (selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.Select();
            lockSelection = true;
            MTR_AudioManager.Instance.PlayMenuHoverEvent();
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void OnPrimaryInteractAction()
    {
        selectableVectorField.CurrentSelection?.InvokeClickAction();
        MTR_AudioManager.Instance.PlayMenuSelectEvent();
    }

    void PlayButtonAction()
    {
        sceneManager.TryGetSceneDataByKnot("scene1_0", out MTRSceneScriptableData scene);
        sceneManager.LoadScene(scene.Name);
    }

    void UnlockSelection()
    {
        lockSelection = false;
    }

    private void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
