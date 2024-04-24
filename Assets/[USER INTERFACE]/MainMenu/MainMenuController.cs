using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public VisualElement ui;
    public Button playButton;
    public Button optionsButton;
    public Button quitButton; 
    public SceneManagerScript gm;  

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("play-button");
        optionsButton = ui.Q<Button>("options-button");
        quitButton = ui.Q<Button>("quit-button");

        playButton.clicked += PlayButtonClicked;
        optionsButton.clicked += OptionsButtonClicked;
        quitButton.clicked += QuitButtonClicked;
    }

    public void PlayButtonClicked()
    {
        gm = FindFirstObjectByType<SceneManagerScript>();
        gm.newSceneName = "MelOMart_Blockout";
        Debug.Log("Play Button Clicked");
    }

    public void OptionsButtonClicked()
    {
        Debug.Log("Options Button Clicked");
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}



