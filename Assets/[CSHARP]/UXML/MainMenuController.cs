using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
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

    //SelectableVectorField<ControlledElement<Button>> selectableVectorField = new SelectableVectorField<Focusable>();
    [SerializeField] int selectablesCount = 0;

    public void Awake()
    {
        Initialize(preset);

        // Load the Selectable Elements
        List<ControlledElement<Button>> selectables = ElementQueryAll<ControlledElement<Button>>();
        selectablesCount = selectables.Count;

        Debug.Log("Selectable Elements Count: " + selectablesCount);

        if (selectables.Count > 0)
        {
            selectables[0].Element.Focus();
        }

    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MainMenuController))]
public class MainMenuControllerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MainMenuController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MainMenuController)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif



