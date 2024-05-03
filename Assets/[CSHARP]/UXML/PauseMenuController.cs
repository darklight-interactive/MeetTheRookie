using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public VisualElement ui;
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("play-button");
        optionsButton = ui.Q<Button>("options-button");
        creditsButton = ui.Q<Button>("credits-button");
        mainMenuButton = ui.Q<Button>("main-menu-button");
        quitButton = ui.Q<Button>("quit-button");

        playButton.clicked += PlayButtonClicked;
        optionsButton.clicked += OptionsButtonClicked;
        creditsButton.clicked += CreditsButtonClicked;
        mainMenuButton.clicked += MainMenuButtonClicked;
        quitButton.clicked += QuitButtonClicked;
    }

    public void PlayButtonClicked()
    {
        Debug.Log("Play Button Clicked");
    }

    public void OptionsButtonClicked()
    {
        Debug.Log("Options Button Clicked");
    }

    public void CreditsButtonClicked()
    {
        Debug.Log("Credits Button Clicked");
    }

    public void MainMenuButtonClicked()
    {
        Debug.Log("Main Menu Button Clicked");
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}



