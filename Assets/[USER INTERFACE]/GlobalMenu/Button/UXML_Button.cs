using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using Ink.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class UXML_Button : MonoBehaviour
{
    public ButtonControls buttons;
    public VisualElement Root;
    public Button button;
    private StyleColor NormalColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f, 1f));
    private StyleScale NormalScale = new Scale(new Vector2(1, 1));
    private StyleColor HoverColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 8f));
    private StyleScale HoverScale = new Scale(new Vector2(1.2f, 1.2f));
    private StyleColor SelectColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 1));
    private StyleScale SelectScale1 = new Scale(new Vector2(1.4f, 1.4f));
    public GMItem item;
    public Sprite sprite;
    public Sprite sprite2;
    // Update is called once per frame
    void Update()
    {
        button = buttons.currentbutton;
        if (Keyboard.current.jKey.isPressed)
        {
            OnSelect();
        }
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            if (buttons.previousbutton.enabledInHierarchy)
            {
                buttons.previousbutton.style.scale = NormalScale;
                buttons.previousbutton.style.backgroundColor = NormalColor;
                buttons.previousbutton.text = "Button";
                item.SetImage(sprite2);
                item.SetText("Label");
                item.SetScale(1);
            }
            OnHover();
        }
    }
    public void OnHover()
    {
        SetStyle("Hover");
    }

    public void OnSelect()
    {
        StartCoroutine(SelectVisual());
        ButtonMethod();
    }
    public void ButtonMethod()
    {
        item.SetImage(sprite);
        item.SetText("Hi :3");
        item.SetScale(0.5f);
    }
        public void SetText(string text)
    {
        button.text = text;
    }
    
    void SetStyle(string style)
    {
        if (style == "Select")
        {
            button.style.backgroundColor = SelectColor;
            SetText("ButtonClicked!");
            button.style.scale = SelectScale1;
        }
        if (style == "Hover")
        {
            button.style.backgroundColor = HoverColor;
            button.style.scale = HoverScale;
        }
    }
    IEnumerator SelectVisual()
    {
        SetStyle("Select");
        yield return new WaitForSeconds(0.3f);
        button.style.scale = HoverScale;
        button.style.backgroundColor = HoverColor;
    }

}
