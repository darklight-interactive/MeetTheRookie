using System.Collections.Generic;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
#endif

/// <summary>
/// This handles all of the Game UI elements like interactions and speech bubbles.
/// </summary>
public class PauseMenuController : UXML_UIDocumentObject
{
    #region ---- < UXML TAGS > ---------------------------------
    const string PAUSE_MENU_CTN = "pausemenu-container";
    const string HOME_PAGE = "home-page";
    const string SETTINGS_PAGE = "settings-page";
    const string CONTROLS_PAGE = "controls-page";
    const string RESUME_BTN = "resume-btn";
    const string SETTINGS_BTN = "settings-btn";
    const string CONTROLS_BTN = "controls-btn";
    const string MAINMENU_BTN = "mainmenu-btn";
    const string RETURN_BTN_SETTINGS = "return-btn-settings";
    const string MUSIC_SLIDER_SETTINGS = "music-slider";
    const string SFX_SLIDER_SETTINGS = "sfx-slider";
    const string DIALOGUE_SLIDER_SETTINGS = "dialogue-slider";
    const string RETURN_BTN_CONTROLS = "return-btn-controls";
    #endregion

    #region ---- < AUDIO BUSSES > ---------------------------------
    const string MUSIC_BUS = "bus:/Music";
    const string SFX_BUS = "bus:/SFX";
    const string DIALOGUE_BUS = "bus:/Dialogue";
    #endregion

    StateMachine _stateMachine;

    /// <summary>
    /// The current selectable elements in the pause menu. This is updated based on the current state.
    /// </summary>
    SelectableVectorField<VisualElement> _selectableVectorField =
        new SelectableVectorField<VisualElement>();

    #region ---- < UI ELEMENT REFERENCES > ---------------------------------
    VisualElement _pauseMenuContainer => ElementQuery<VisualElement>(PAUSE_MENU_CTN);
    VisualElement _homePage => ElementQuery<VisualElement>(HOME_PAGE);
    VisualElement _controlsPage => ElementQuery<VisualElement>(CONTROLS_PAGE);
    VisualElement _settingsPage => ElementQuery<VisualElement>(SETTINGS_PAGE);
    SelectableButton _resumeButton => ElementQuery<SelectableButton>(RESUME_BTN);
    SelectableButton _settingsPageButton => ElementQuery<SelectableButton>(SETTINGS_BTN);
    SelectableButton _controlsPageButton => ElementQuery<SelectableButton>(CONTROLS_BTN);
    SelectableButton _mainMenuButton => ElementQuery<SelectableButton>(MAINMENU_BTN);
    SelectableButton _settingsReturnButton => ElementQuery<SelectableButton>(RETURN_BTN_SETTINGS);
    SelectableSlider _settingsMusicSlider => ElementQuery<SelectableSlider>(MUSIC_SLIDER_SETTINGS);
    SelectableSlider _settingsSFXSlider => ElementQuery<SelectableSlider>(SFX_SLIDER_SETTINGS);
    SelectableSlider _settingsDialogueSlider =>
        ElementQuery<SelectableSlider>(DIALOGUE_SLIDER_SETTINGS);
    SelectableButton _controlsReturnButton => ElementQuery<SelectableButton>(RETURN_BTN_CONTROLS);
    Dictionary<PauseMenuState, VisualElement[]> _pageElementGroups =>
        new Dictionary<PauseMenuState, VisualElement[]>()
        {
            { PauseMenuState.NONE, new VisualElement[] { } },
            {
                PauseMenuState.HOME,
                new VisualElement[]
                {
                    _resumeButton,
                    _settingsPageButton,
                    _controlsPageButton,
                    _mainMenuButton
                }
            },
            {
                PauseMenuState.SETTINGS,
                new VisualElement[]
                {
                    _settingsReturnButton,
                    _settingsMusicSlider,
                    _settingsSFXSlider,
                    _settingsDialogueSlider
                }
            },
            { PauseMenuState.CONTROLS, new VisualElement[] { _controlsReturnButton } }
        };

    #endregion


    [Button]
    public void OpenHomePage() => _stateMachine.GoToState(PauseMenuState.HOME);

    [Button]
    public void OpenSettingsPage() => _stateMachine.GoToState(PauseMenuState.SETTINGS);

    [Button]
    public void OpenControlsPage() => _stateMachine.GoToState(PauseMenuState.CONTROLS);

    public void Awake()
    {
        Initialize(preset);

        MTRInputManager.OnMenuButton += OnMenuButtonAction;
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        _pauseMenuContainer.style.visibility = Visibility.Hidden;

        _stateMachine = new StateMachine(this);
        _stateMachine.GoToState(PauseMenuState.NONE);
    }

    public void Start()
    {
        _resumeButton.OnClick += OnMenuButtonAction;

        _settingsPageButton.OnClick += () => _stateMachine.GoToState(PauseMenuState.SETTINGS);

        _controlsPageButton.OnClick += () => _stateMachine.GoToState(PauseMenuState.CONTROLS);

        _mainMenuButton.OnClick += () => Debug.Log("Main menu button clicked");

        _settingsReturnButton.OnClick += () => _stateMachine.GoToState(PauseMenuState.HOME);

        _controlsReturnButton.OnClick += () => _stateMachine.GoToState(PauseMenuState.HOME);

        // << SLIDERS >>
        _settingsMusicSlider.value = MTR_AudioManager.Instance.GetBus(MUSIC_BUS).Volume;
        _settingsSFXSlider.value = MTR_AudioManager.Instance.GetBus(SFX_BUS).Volume;
        _settingsDialogueSlider.value = MTR_AudioManager.Instance.GetBus(DIALOGUE_BUS).Volume;

        _settingsMusicSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(MUSIC_BUS, _settingsMusicSlider.value);
        };

        _settingsSFXSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(SFX_BUS, _settingsSFXSlider.value);
        };

        _settingsDialogueSlider.OnValueChanged += () =>
        {
            MTR_AudioManager.Instance.SetBusVolume(DIALOGUE_BUS, _settingsDialogueSlider.value);
        };
    }

    void OnDestroy()
    {
        MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
        MTRInputManager.OnMenuButton -= OnMenuButtonAction;
    }

    #region ---- < INPUT HANDLING > ---------------------------------
    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        //RotateChoiceSelection((int)directionInScreenSpace.y);

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
        }
        else if (newButton is SelectableButton newSelectableButton)
        {
            newSelectableButton.Select();
        }

        if (directionInScreenSpace.y != 0.0 && oldButton != newButton)
        {
            MTR_AudioManager.Instance.PlayMenuHoverEvent();
        }
    }

    void OnPrimaryInteractAction()
    {
        /*
        if (_choicePanel.visible)
        {
            _choiceButtons[selectedChoiceIndex].InvokeClickAction();
        }
        */

        if (_pauseMenuContainer.visible)
        {
            MTR_AudioManager.Instance.PlayMenuSelectEvent();
        }
        if (_selectableVectorField.CurrentSelection is SelectableButton button)
        {
            button.InvokeClickAction();
        }
    }

    void OnMenuButtonAction()
    {
        Debug.Log("[PauseMenuController] OnMenuButtonAction");

        // << RESUME GAME >>
        if (_stateMachine.CurrentState != PauseMenuState.NONE)
        {
            _stateMachine.GoToState(PauseMenuState.NONE);
            MTRSceneController.StateMachine.GoToState(MTRSceneState.PLAY_MODE);

            MTRInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
        }
        // << OPEN PAUSE MENU >>
        else
        {
            if (MTRSceneController.StateMachine.CurrentState != MTRSceneState.PLAY_MODE)
                return;

            _stateMachine.GoToState(PauseMenuState.HOME);
            MTRSceneController.StateMachine.GoToState(MTRSceneState.PAUSE_MODE);

            // Listen to the input manager
            MTRInputManager.OnMoveInputStarted += OnMoveInputStartAction;
            MTRInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
        }
    }
    #endregion

    #region ---- < HELPER METHODS > ---------------------------------

    /// <summary>
    /// Hides all of the pages in the pause menu.
    /// </summary>
    void SetAllElementsToHidden()
    {
        _pauseMenuContainer.style.visibility = Visibility.Hidden;
        _homePage.style.visibility = Visibility.Hidden;
        _controlsPage.style.visibility = Visibility.Hidden;
        _settingsPage.style.visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Shows the page for the given state. This is called when the state is changed and sets the visibility of the page and all other pages.
    /// </summary>
    /// <param name="state">The state to show the page for.</param>
    void ShowPage(PauseMenuState state)
    {
        if (state == PauseMenuState.NONE)
        {
            SetAllElementsToHidden();
            return;
        }

        _pauseMenuContainer.style.visibility = Visibility.Visible;
        _homePage.style.visibility =
            state == PauseMenuState.HOME ? Visibility.Visible : Visibility.Hidden;
        _controlsPage.style.visibility =
            state == PauseMenuState.CONTROLS ? Visibility.Visible : Visibility.Hidden;
        _settingsPage.style.visibility =
            state == PauseMenuState.SETTINGS ? Visibility.Visible : Visibility.Hidden;
    }

    /// <summary>
    /// Loads the selectable elements for the given state to the selectable vector field.
    /// </summary>
    /// <param name="state">The state to load the selectable elements for.</param>
    void LoadSelectableElements(PauseMenuState state)
    {
        _selectableVectorField.Clear();

        // If there are any elements in the group, load them.
        if (_pageElementGroups[state].Length > 0)
            _selectableVectorField.Load(_pageElementGroups[state]);
    }

    #endregion

    #region ---- < STATE MACHINE > ---------------------------------
    enum PauseMenuState
    {
        NONE,
        HOME,
        SETTINGS,
        CONTROLS
    }

    class StateMachine : FiniteStateMachine<PauseMenuState>
    {
        protected PauseMenuController controller;

        public StateMachine(PauseMenuController controller)
        {
            this.controller = controller;
            possibleStates = new Dictionary<PauseMenuState, FiniteState<PauseMenuState>>
            {
                { PauseMenuState.NONE, new NoneState(this) },
                { PauseMenuState.HOME, new HomeState(this) },
                { PauseMenuState.SETTINGS, new SettingsState(this) },
                { PauseMenuState.CONTROLS, new ControlsState(this) }
            };
        }

        class BaseState : FiniteState<PauseMenuState>
        {
            protected PauseMenuController _controller;

            public BaseState(StateMachine stateMachine, PauseMenuState stateType)
                : base(stateMachine, stateType)
            {
                _controller = stateMachine.controller;
            }

            public override void Enter()
            {
                Debug.Log($"[PauseMenuController] {StateType} Enter");
                _controller.ShowPage(StateType);
                _controller.LoadSelectableElements(StateType);
            }

            public override void Execute() { }

            public override void Exit() { }
        }

        class NoneState : BaseState
        {
            public NoneState(StateMachine stateMachine)
                : base(stateMachine, PauseMenuState.NONE) { }
        }

        class HomeState : BaseState
        {
            public HomeState(StateMachine stateMachine)
                : base(stateMachine, PauseMenuState.HOME) { }
        }

        class SettingsState : BaseState
        {
            public SettingsState(StateMachine stateMachine)
                : base(stateMachine, PauseMenuState.SETTINGS) { }
        }

        class ControlsState : BaseState
        {
            public ControlsState(StateMachine stateMachine)
                : base(stateMachine, PauseMenuState.CONTROLS) { }
        }
    }
    #endregion
}
