using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StackSynthUI : MonoBehaviour
{
    public VisualElement root;
    public VisualElement stackBase;
    public List<VisualElement> Stack = new List<VisualElement>();
    private int childcounter = 0;
    private string j;
    private bool selected = false;
    private bool hovered = false;
    private bool textselected = false;
    private VisualElement holder;
    private VisualElement CurrentFile;
    private VisualElement CurrentTextHolder;
    private List<Label> Lines = new List<Label>();
    private Label CurrentLine;
    private Label LineChangeHolder;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        stackBase = root.Q<VisualElement>("Stack");
        stackBase.style.scale = new StyleScale(new Vector2(1.4f, 1.4f));
        foreach (VisualElement child in stackBase.Children())
            {
                Stack.Add(child);
                childcounter += 1;
                child.style.top = 20 * childcounter;
                child.style.left = 20 * childcounter;
            }
        childcounter = 0;
        AddText("rat", "yomama");
        AddText("rat", "I'm a rat");
        AddText("rat", "shiminy");
        AddText("rat", "biminy");
        AddText("rat", "bop");
        AddText("rat", "the kids went down to the store");
        AddText("rat", "hi my name is bob and I approve this message");
        AddText("dog", "yomama");
        AddText("dog", "I'm a rat");
        AddText("dog", "shiminy");
        AddText("dog", "biminy");
        AddText("dog", "bop");
        AddText("dog", "the kids went down to the store");
        AddText("dog", "hi my name is bob and I approve this message");
        TextFix(Stack[^1].name);
        //Make sure to comment this out later!
        OnHover();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentFile = Stack[^1];
        if (hovered == true || selected == true || textselected == true)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Select();
            }
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                Deselect();
            }
            if (selected == true)
            {
                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    Change();
                }
            }
            if (textselected == true)
            {
                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    TextChange();
                }
            }
        }
    }

    public void Select()
    {
        if (textselected == true)
        {
            GrabText();
        }
        if (selected == true)
        {
            selected = false;
            textselected = true;
            CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            CurrentFile.Q<VisualElement>("TextSelected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
        if (hovered == true)
        {
            hovered = false;
            selected = true;
            CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
    }
    public void Deselect()
    {
        if (selected == true)
        {
            selected = false;
            hovered = true;
            CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
        if (textselected == true)
        {
            textselected = false;
            selected = true;
            CurrentFile.Q<VisualElement>("TextSelected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
    }
    
    public void OnHover()
    {
        hovered = !hovered;
        if (hovered == true)
            {
                CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
            }
        if (hovered == false)
            {
                CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            }
    }

    public void Change()
    {
        CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
        holder = Stack[0];
        Stack.Remove(Stack[0]);
        Stack.Add(holder);
        foreach (VisualElement child in Stack)
        {   
            Stack[childcounter].BringToFront();
            child.style.backgroundColor = new StyleColor(new Color(0.72f - 0.5f/(childcounter+1), 0.591049f - 0.4f/(childcounter+1), 0.2371274f - 0.12f/(childcounter+1), 1));
            childcounter += 1;
            child.style.top = 20 * childcounter;
            child.style.left = 20 * childcounter;
        }
        childcounter = 0;
        CurrentFile = holder;
        CurrentFile.Q<VisualElement>("Selected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        TextFix(CurrentFile.name);
    }

    public void TextFix(string text)
    {
        VisualElement fix = stackBase.Q<VisualElement>(text);
        Lines.Clear();
        CurrentTextHolder = fix.Q<VisualElement>("TextHolder");
        int linescounter = 0;
        foreach (Label child in CurrentTextHolder.Children())
        {
            Lines.Add(child);
            linescounter += 1;
            child.style.top = -4 + 30 * (linescounter-1);
            if (child.style.top.value.value > 60)
            {
                child.style.color = Color.clear;
            }
            else
            {
                child.style.color = Color.black;
            }
        }
        linescounter = 0;
        CurrentLine = Lines[0];
    }
    public void GrabText()
    {
        Debug.Log(CurrentLine.text + "grabbed!");
    }
    public void TextChange()
    {
        int linescounter = 0;
        LineChangeHolder = Lines[0];
        Lines.Remove(Lines[0]);
        Lines.Add(LineChangeHolder);
        foreach (Label child in Lines)
        {
            linescounter += 1;
            child.style.top = -4 + 30 * (linescounter-1);
            if (child.style.top.value.value > 60)
            {
                child.style.color = Color.clear;
            }
            else
            {
                child.style.color = Color.black;
            }
        }
        linescounter = 0;
        CurrentLine = Lines[0];
    }
    public void AddText(string filename, string LabelText)
    {
        VisualElement file = stackBase.Q<VisualElement>(filename);
        Label label = new Label(LabelText);
        label.AddToClassList("StackText");
        if (LabelText.Length > 16)
        {
            label.style.fontSize = 12;
        }
        if (LabelText.Length > 32)
        {
            label.style.fontSize = 10;
        }
        VisualElement textholder = file.Q<VisualElement>("TextHolder");
        textholder.Add(label);
        TextFix(filename);
    }
}
