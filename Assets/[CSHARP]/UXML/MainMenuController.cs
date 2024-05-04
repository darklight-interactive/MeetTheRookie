using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.UXML.Element;
using System.Collections.Generic;
using System;
using Darklight.UnityExt.Scene;



#if UNITY_EDITOR
using UnityEditor;
#endif


public class MainMenuController : UXML_UIDocumentObject
{
    /*
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
    */

    SelectableVectorField<SelectableElement> selectableVectorField = new SelectableVectorField<SelectableElement>();
    [SerializeField] int selectablesCount = 0;

    void Awake()
    {
        Initialize(preset);

        // Load the Selectable Elements
        List<SelectableElement> selectables = ElementQueryAll<SelectableElement>();
        selectablesCount = selectables.Count;

    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}



