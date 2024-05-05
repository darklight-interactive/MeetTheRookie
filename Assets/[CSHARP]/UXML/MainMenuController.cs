using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Input;
using System.Linq;
using Darklight.Game.Selectable;
using Darklight.UXML;
using Darklight.Selectable;




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

    SelectableVectorField<SelectableButton> selectableVectorField = new SelectableVectorField<SelectableButton>();
    [SerializeField] int selectablesCount = 0;

    bool lockSelection = false;

    public void Awake()
    {
        Initialize(preset);

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        Select(selectableVectorField.Selectables.First());

        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += (Vector2 dir) =>
        {
            Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y
            SelectableButton buttonInDirection = selectableVectorField.getFromDir(directionInScreenSpace);
            Select(buttonInDirection);
        };

        UniversalInputManager.OnPrimaryInteract += () =>
        {
            selectableVectorField.CurrentSelection.Click();
        };


    }

    void Select(SelectableButton selectedButton)
    {
        SelectableButton previousButton = selectableVectorField.PreviousSelection;
        if (!lockSelection && selectedButton != null && selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.Select();
            LockSelection();
        }
    }

    void LockSelection()
    {
        lockSelection = true;
        Invoke("UnlockSelection", 0.1f);
    }

    void UnlockSelection()
    {
        lockSelection = false;
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

    private void OnDisable()
    {
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



