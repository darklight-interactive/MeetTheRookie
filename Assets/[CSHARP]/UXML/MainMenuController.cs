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

    public void Awake()
    {
        Initialize(preset);

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());

        // Listen to the input manager
        UniversalInputManager.OnMoveInput += (Vector2 dir) =>
        {
            SelectableButton previousButton = selectableVectorField.CurrentSelection;
            SelectableButton selected = selectableVectorField.getFromDir(dir * -1); // inverted y
            if (previousButton != null)
            {
                previousButton.RemoveFromClassList("selected");
                selected.AddToClassList("selected");
                selected.Focus();
                Debug.Log($"Selected: {selected.name}");
            }
        };

        UniversalInputManager.OnPrimaryInteract += () =>
        {
            selectableVectorField.CurrentSelection.Activate();
        };
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



