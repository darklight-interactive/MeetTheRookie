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
    SelectableButton _playButton;
    SelectableButton _settingsButton;
    SelectableButton _creditsButton;
    SelectableButton _quitButton;
    //SelectableButton _scene3Button;
    //SelectableButton _scene4Button;
    SelectableSlider _musicSlider;
    SelectableSlider _sfxSlider;
    SelectableSlider _dialogueSlider;
    SelectableButton _returnButtonSettings;
    SelectableButton _returnButtonCredits;
    SelectableButton _currentButton;
    Dictionary<string, SelectableButton[]> buttonGroups;
    bool lockSelection = false;

    [SerializeField]
    SelectableVectorField<VisualElement> _selectableVectorField =
        new SelectableVectorField<VisualElement>();

    const string MAIN_PAGE = "main";
    const string SETTINGS_PAGE = "main-settings";
    const string CREDITS_PAGE = "main-credits";
    VisualElement _settingsPage;
    VisualElement _creditsPage;

    public void Awake()
    {
        Initialize(preset);
    }

    void Start()
    {
        // Store the local references to the buttons
        _playButton = ElementQuery<SelectableButton>("play-btn");
        _settingsButton = ElementQuery<SelectableButton>("settings-btn");
        _creditsButton = ElementQuery<SelectableButton>("credits-btn");
        _quitButton = ElementQuery<SelectableButton>("quit-btn");
        //_scene3Button = ElementQuery<SelectableButton>("scene3-btn");
        //_scene4Button = ElementQuery<SelectableButton>("scene4-btn");
        _musicSlider = ElementQuery<SelectableSlider>("music-slider");
        _sfxSlider = ElementQuery<SelectableSlider>("sfx-slider");
        _dialogueSlider = ElementQuery<SelectableSlider>("dialogue-slider");
        _returnButtonSettings = ElementQuery<SelectableButton>("return-btn-settings");
        _returnButtonCredits = ElementQuery<SelectableButton>("return-btn-credits");

        buttonGroups = new Dictionary<string, SelectableButton[]>
        {
            {
                MAIN_PAGE,
                new SelectableButton[] { _playButton, _settingsButton, _creditsButton, _quitButton }
            },
            { SETTINGS_PAGE, new SelectableButton[] { _returnButtonSettings } },
            { CREDITS_PAGE, new SelectableButton[] { _returnButtonCredits } }
        };

        // Load the selectable buttons, remove elements not currently active in the page
        _selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        _selectableVectorField.RemoveRange(buttonGroups[SETTINGS_PAGE]);
        _selectableVectorField.RemoveRange(buttonGroups[CREDITS_PAGE]);

        // Store references to the folders
        _settingsPage = ElementQuery<VisualElement>(SETTINGS_PAGE);
        _creditsPage = ElementQuery<VisualElement>(CREDITS_PAGE);

        // Assign the events
        _playButton.OnClick += PlayButtonAction;
        _settingsButton.OnClick += () =>
        {
            _selectableVectorField.AddRange(ElementQueryAll<SelectableSlider>());
            _selectableVectorField.AddRange(buttonGroups[SETTINGS_PAGE]);
            _selectableVectorField.RemoveRange(buttonGroups[MAIN_PAGE]);

            _settingsPage.style.display = DisplayStyle.Flex;
            _settingsPage.AddToClassList("visible");

            _selectableVectorField.Reset();
            _selectableVectorField.Select(_musicSlider);
        };

        _returnButtonSettings.OnClick += () =>
        {
            _selectableVectorField.AddRange(buttonGroups[MAIN_PAGE]);
            _selectableVectorField.RemoveRange(ElementQueryAll<SelectableSlider>());
            _selectableVectorField.RemoveRange(buttonGroups[SETTINGS_PAGE]);

            _settingsPage.RemoveFromClassList("visible");
            _settingsPage.style.display = DisplayStyle.None;

            _selectableVectorField.Reset();
            _selectableVectorField.Select(_settingsButton);
        };

        _creditsButton.OnClick += () =>
        {
            _selectableVectorField.AddRange(ElementQueryAll<Scroller>());
            _selectableVectorField.AddRange(buttonGroups[CREDITS_PAGE]);
            _selectableVectorField.RemoveRange(buttonGroups[MAIN_PAGE]);

            _creditsPage.style.display = DisplayStyle.Flex;
            _creditsPage.AddToClassList("visible");

            _selectableVectorField.Reset();
            _selectableVectorField.Select(_returnButtonCredits);
        };

        _returnButtonCredits.OnClick += () =>
        {
            _selectableVectorField.RemoveRange(ElementQueryAll<Scroller>());
            _selectableVectorField.RemoveRange(buttonGroups[CREDITS_PAGE]);
            _selectableVectorField.AddRange(buttonGroups[MAIN_PAGE]);

            _creditsPage.RemoveFromClassList("visible");
            _creditsPage.style.display = DisplayStyle.None;

            _selectableVectorField.Reset();
            _selectableVectorField.Select(_creditsButton);
        };

        _quitButton.OnClick += Quit;

        //_scene3Button.OnClick += () =>
        //{
        //    MTRSceneManager.Instance.TryGetSceneDataByKnot("scene3_1", out MTRSceneData scene);
        //    MTRSceneController.Instance.TryLoadScene(scene.Name);
        //};

        //_scene4Button.OnClick += () =>
        //{
        //    MTRSceneManager.Instance.TryGetSceneDataByKnot("scene4_1", out MTRSceneData scene);
        //    MTRSceneController.Instance.TryLoadScene(scene.Name);
        //};

        // Sliders
        _musicSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Music", _musicSlider.value);
        };
        Debug.Log("MUSIC: " + _musicSlider.value);

        _sfxSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/SFX", _sfxSlider.value);
        };

        _dialogueSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume("bus:/Dialogue", _dialogueSlider.value);
        };

        _musicSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Music").Volume;
        _sfxSlider.value = MTR_AudioManager.Instance.GetBus("bus:/SFX").Volume;
        _dialogueSlider.value = MTR_AudioManager.Instance.GetBus("bus:/Dialogue").Volume;

        // Listen to the input manager
        MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space

        if (_selectableVectorField.CurrentSelection is SelectableSlider slider)
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
        VisualElement oldButton = _selectableVectorField.CurrentSelection;
        if (oldButton is SelectableSlider oldSlider)
        {
            oldSlider.Deselect();
        }
        else if (oldButton is SelectableButton button)
        {
            button.Deselect();
        }

        VisualElement newButton = _selectableVectorField.SelectElementInDirection(
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
            _currentButton = newSelectableButton;
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
        SelectableButton previousButton = _currentButton;
        if (selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.Select();
            _currentButton = selectedButton;
            lockSelection = true;
            MTR_AudioManager.Instance.PlayMenuHoverEvent();
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void OnPrimaryInteractAction()
    {
        //selectableVectorField.CurrentSelection?.InvokeClickAction();

        if (_selectableVectorField.CurrentSelection is SelectableButton button)
        {
            button.InvokeClickAction();
            MTR_AudioManager.Instance.PlayMenuSelectEvent();
        }
    }

    void PlayButtonAction()
    {
        MTRSceneManager.Instance.TryGetSceneDataByKnot("scene1_0", out MTRSceneData scene);
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
