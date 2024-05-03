using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class MainMenuController : UXML_UIDocumentObject
{
    [System.Serializable]
    public class SceneButtonData
    {
        public string tag;
        public SceneObject scene;

        // -- EVENTS -- >>
        public delegate void OnClick(SceneObject scene);
        public event OnClick OnButtonClicked;

        // Internal Data
        UXML_UIDocumentObject documentObject;
        Button button;

        public void Initialize(UXML_UIDocumentObject documentObject)
        {
            this.documentObject = documentObject;
            button = documentObject.GetUIElement(tag).element as Button;

            button.clickable.clicked += () =>
            {
                OnButtonClicked?.Invoke(scene);
            };
        }
    }

    Dictionary<string, SceneButtonData> sceneButtons = new Dictionary<string, SceneButtonData>();
    public List<SceneButtonData> sceneButtonList = new List<SceneButtonData>();

    void Awake()
    {
        Initialize(preset);
        LoadSceneButtons();
    }

    void LoadSceneButtons()
    {
        foreach (SceneButtonData sceneButtonData in sceneButtonList)
        {
            sceneButtonData.Initialize(this);
            sceneButtons.Add(sceneButtonData.tag, sceneButtonData);

            sceneButtonData.OnButtonClicked += (scene) =>
            {
                SceneManager.Instance.LoadScene(scene);

                // Clear the event
                sceneButtonData.OnButtonClicked -= (scene) => { };
            };
        }
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}



