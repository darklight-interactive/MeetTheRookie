using System.Collections.Generic;
using System.Linq;
//using System.Collections.IEnumerable;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : UXML_UIDocumentObject
{
    MTRSceneManager sceneManager;
    SelectableButton playButton;
    SelectableButton settingsButton;
    SelectableButton creditsButton;
    SelectableButton quitButton;
    SelectableButton returnButtonSettings;
    SelectableButton returnButtonCredits;
    SelectableButton currentButton;
    Dictionary<string, SelectableButton[]> buttonGroups;
    SelectableVectorField<SelectableButton> selectableVectorField =
        new SelectableVectorField<SelectableButton>();
    bool lockSelection = false;

    SelectableVectorField<VisualElement> selectableElements =
        new SelectableVectorField<VisualElement>();

    const string MAIN_PAGE = "main";
    const string SETTINGS_PAGE = "main-settings";
    const string CREDITS_PAGE = "main-credits";
    VisualElement _settingsPage;
    VisualElement _creditsPage;

    public void Awake()
    {
        sceneManager = MTRSceneManager.Instance as MTRSceneManager;
        Initialize(preset);
    }

    void Start()
    {
        // Store the local references to the buttons
        playButton = ElementQuery<SelectableButton>("play-btn");
        settingsButton = ElementQuery<SelectableButton>("settings-btn");
        creditsButton = ElementQuery<SelectableButton>("credits-btn");
        quitButton = ElementQuery<SelectableButton>("quit-btn");
        returnButtonSettings = ElementQuery<SelectableButton>("return-btn-settings");
        returnButtonCredits = ElementQuery<SelectableButton>("return-btn-credits");

        buttonGroups = new Dictionary<string, SelectableButton[]>
        {
            {
                MAIN_PAGE,
                new SelectableButton[] { playButton, settingsButton, creditsButton, quitButton }
            },
            { SETTINGS_PAGE, new SelectableButton[] { returnButtonSettings } },
            { CREDITS_PAGE, new SelectableButton[] { returnButtonCredits } }
        };

        // Load the selectable buttons, remove elements not currently active in the page
        selectableElements.Load(ElementQueryAll<SelectableButton>());
        selectableElements.RemoveRange(buttonGroups[SETTINGS_PAGE]);
        selectableElements.RemoveRange(buttonGroups[CREDITS_PAGE]);

        // Store references to the folders
        _settingsPage = ElementQuery<VisualElement>(SETTINGS_PAGE);
        _creditsPage = ElementQuery<VisualElement>(CREDITS_PAGE);

        // Assign the events
        playButton.OnClick += PlayButtonAction;
        //settingsButton.OnClick += () => Debug.Log("Options Button Clicked");
        //creditsButton.OnClick += () => Debug.Log("Credits Button Clicked");
        settingsButton.OnClick += () =>
        {
            selectableElements.AddRange(ElementQueryAll<SelectableSlider>());
            selectableElements.AddRange(buttonGroups[SETTINGS_PAGE]);
            selectableElements.RemoveRange(buttonGroups[MAIN_PAGE]);

            //_settingsPage.style.display = DisplayStyle.Flex;
            //_settingsPage.visible = true;
            _settingsPage.AddToClassList("visible");
        };

        returnButtonSettings.OnClick += () =>
        {
            selectableElements.AddRange(buttonGroups[MAIN_PAGE]);
            selectableElements.RemoveRange(ElementQueryAll<SelectableSlider>());
            selectableElements.RemoveRange(buttonGroups[SETTINGS_PAGE]);

            _settingsPage.RemoveFromClassList("visible");
            //_settingsPage.style.display = DisplayStyle.None;
            //_settingsPage.visible = false;
        };

        creditsButton.OnClick += () =>
        {
            selectableElements.AddRange(ElementQueryAll<Scroller>());
            selectableElements.AddRange(buttonGroups[CREDITS_PAGE]);
            selectableElements.RemoveRange(buttonGroups[MAIN_PAGE]);

            //_creditsPage.style.display = DisplayStyle.Flex;
            //_creditsPage.visible = true;
            _creditsPage.AddToClassList("visible");
        };

        returnButtonCredits.OnClick += () =>
        {
            selectableElements.RemoveRange(ElementQueryAll<Scroller>());
            selectableElements.RemoveRange(buttonGroups[CREDITS_PAGE]);
            selectableElements.AddRange(buttonGroups[MAIN_PAGE]);

            _creditsPage.RemoveFromClassList("visible");
            //_creditsPage.style.display = DisplayStyle.None;
            //_creditsPage.visible = false;
        };

        quitButton.OnClick += Quit;

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        selectableVectorField.Selectables.First().Select();
        currentButton = selectableVectorField.Selectables.First();

        // Sliders
        SelectableSlider musicSlider = ElementQuery<SelectableSlider>("music-slider");
        musicSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Music", musicSlider.value);
        };
        Debug.Log("MUSIC: " + musicSlider.value);

        SelectableSlider sfxSlider = ElementQuery<SelectableSlider>("sfx-slider");
        sfxSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/SFX", sfxSlider.value);
        };

        SelectableSlider dialogueSlider = ElementQuery<SelectableSlider>("dialogue-slider");
        dialogueSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Dialogue", dialogueSlider.value);
        };

        musicSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Music").Volume;
        sfxSlider.value = MTR_AudioManager.Instance.GetBus("bus:/SFX").Volume;
        dialogueSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Dialogue").Volume;

        // Listen to the input manager
        MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space

        if (selectableElements.CurrentSelection is SelectableSlider slider)
        {
            if (dir.y == 0)
            {
                if (dir.x > 0)
                {
                    slider.Increment();
                    MTR_AudioManager.Instance.PlayMenuSliderEvent();
                }
                else if (dir.x < 0)
                {
                    slider.Decrement();
                    MTR_AudioManager.Instance.PlayMenuSliderEvent();
                }
                return;
            }
        }

        // Select the next button in the direction
        VisualElement oldButton = selectableElements.CurrentSelection;
        if (oldButton is SelectableSlider oldSlider)
        {
            oldSlider.Deselect();
        }
        else if (oldButton is SelectableButton button)
        {
            button.Deselect();
        }

        VisualElement newButton = selectableElements.SelectElementInDirection(
            directionInScreenSpace
        );
        if (newButton is SelectableSlider newSlider)
        {
            newSlider.Select();
            lockSelection = true;
            Invoke(nameof(UnlockSelection), 0.1f);
        }
        else if (newButton is SelectableButton newSelectableButton)
        {
            newSelectableButton.Select();
            currentButton = newSelectableButton;
            lockSelection = true;
            Invoke(nameof(UnlockSelection), 0.1f);
        }

        if (directionInScreenSpace.y != 0.0 && oldButton != newButton)
        {
            MTR_AudioManager.Instance.PlayMenuHoverEvent();
        }
        //SelectableButton button = selectableVectorField.SelectElementInDirection(directionInScreenSpace);
        //Select(button);
        //Debug.Log($"MainMenuController: OnMoveInputStartAction({dir}) -> {button.name}");
    }

    // UNUSED AFTER ADDING SLIDERS TO THE MENU
    void Select(SelectableButton selectedButton)
    {
        if (selectedButton == null || lockSelection)
            return;

        //SelectableButton previousButton = selectableVectorField.PreviousSelection;
        SelectableButton previousButton = currentButton;
        if (selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.Select();
            currentButton = selectedButton;
            lockSelection = true;
            MTR_AudioManager.Instance.PlayMenuHoverEvent();
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void OnPrimaryInteractAction()
    {
        //selectableVectorField.CurrentSelection?.InvokeClickAction();

        if (selectableElements.CurrentSelection is SelectableButton button)
        {
            button.InvokeClickAction();
            MTR_AudioManager.Instance.PlayMenuSelectEvent();
        }
    }

    void PlayButtonAction()
    {
        sceneManager.TryGetSceneDataByKnot("scene1_0", out MTRSceneData scene);
        MTRSceneController.Instance.TryLoadScene(scene.Name);
    }

    void UnlockSelection()
    {
        lockSelection = false;
    }

    private void OnDestroy()
    {
        MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
