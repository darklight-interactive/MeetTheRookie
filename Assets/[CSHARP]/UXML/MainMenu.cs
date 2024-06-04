using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using System.Linq;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Input;

public class MainMenu : UXML_UIDocumentObject
{
    public Button PlayButton;
    public Button OptionsButton;
    public Button QuitButton;
    public int selector;
    public List<Button> Buttons = new List<Button>();
    SelectableVectorField<Button> selectableVectorField = new SelectableVectorField<Button>();
    bool lockSelection = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayButton = ElementQuery<Button>("Play");
        OptionsButton = ElementQuery<Button>("Options");
        QuitButton = ElementQuery<Button>("Quit");

        Buttons = ElementQueryAll<Button>().ToList();
        selectableVectorField.Load(Buttons);

        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;

        PlayButton.clicked += () =>
        {
            Debug.Log("Play Button Clicked");
            //string scene1_0_name = MTR_SceneManager.Instance.
        };
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        Button buttonInDirection = selectableVectorField.getFromDir(directionInScreenSpace);
        Select(buttonInDirection);
        Debug.Log($"Move Input Started {buttonInDirection.name}");
    }

    void OnPrimaryInteractAction()
    {
        // selectableVectorField.CurrentSelection?.Click();
    }


    void Select(Button selectedButton)
    {
        if (selectedButton == null || lockSelection) return;
        selectedButton.AddToClassList("Selected");

        Button previousButton = selectableVectorField.PreviousSelection;
        if (previousButton != null)
            previousButton.RemoveFromClassList("Selected");

        if (selectedButton != previousButton)
        {
            lockSelection = true;
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void UnlockSelection()
    {
        lockSelection = false;
    }

    private void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow) && selector > 0)
        {
            Buttons[selector].style.fontSize = 70;
            selector -= 1;
            Buttons[selector].style.fontSize = 85;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && selector < 2)
        {
            Buttons[selector].style.fontSize = 70;
            selector += 1;
            Buttons[selector].style.fontSize = 85;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Select();
        }
        */
    }
    void Select()
    {

    }
}
