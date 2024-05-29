using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SynthStackUI : MonoBehaviour
{
    public VisualElement root;
    public VisualElement stackBase;
    public List<VisualElement> Stack = new List<VisualElement>();
    private int childcounter = 0;
    private float colorchanger;
    public bool hovered = false;
    public bool nothovered = true;
    public bool textselected = false;
    private VisualElement holder;
    public VisualElement CurrentFile;
    private VisualElement CurrentTextHolder;
    private List<Label> Lines = new List<Label>();
    public Label CurrentLine;
    private Label LineChangeHolder;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        stackBase = root.Q<VisualElement>("Stack");
        foreach (VisualElement child in stackBase.Children())
            {
                Stack.Add(child);
                childcounter += 1;
                child.style.top = 20 * childcounter;
                child.style.left = 20 * childcounter;
                colorchanger = (childcounter+1) * 2.5f/10;
                child.style.unityBackgroundImageTintColor = new StyleColor(new Color(colorchanger, colorchanger, colorchanger, 1));
            }
        childcounter = 0;
        TextFix(Stack[^1].name);
        CurrentFile = Stack[^1];
        //Make sure to comment this out later!
    }

    public void Select()
    {
        if (Lines.Count > 0)
        {
            if (textselected == true)
            {
                GrabText();
            }
            if (hovered == true)
            {
                hovered = false;
                textselected = true;
                CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
                CurrentFile.Q<VisualElement>("TextSelected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
            }
        }
    }
    public void Deselect()
    {
        if (textselected == true)
        {
            textselected = false;
            hovered = true;
            CurrentFile.Q<VisualElement>("TextSelected").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
    }
    
    public void OnHover()
    {
        hovered = !hovered;
        nothovered = !nothovered;
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
        Debug.Log("yomama");
        CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
        holder = Stack[0];
        Stack.Remove(Stack[0]);
        Stack.Add(holder);
        foreach (VisualElement child in Stack)
        {   
            Stack[childcounter].BringToFront();
            colorchanger = ((childcounter+1) * 2.5f)/10;
            child.style.unityBackgroundImageTintColor = new StyleColor(new Color(colorchanger, colorchanger, colorchanger, 1));
            childcounter += 1;
            child.style.top = 20 * childcounter;
            child.style.left = 20 * childcounter;
        }
        childcounter = 0;
        CurrentFile = holder;
        CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
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
            if (child.style.top.value.value > 90)
            {
                child.style.color = Color.clear;
            }
            else
            {
                child.style.color = Color.black;
            }
        }
        linescounter = 0;
        if (Lines.Count > 0)
        {
            CurrentLine = Lines[0];
        }
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
            if (child.style.top.value.value > 90)
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
    public void SetScale(float xdir, float ydir)
    {
        stackBase.style.scale = new StyleScale(new Scale(new Vector2(xdir, ydir)));
    }
    public void Move(float xdir, float ydir)
    {
        Vector2 move = new Vector2(xdir, ydir);
        stackBase.style.top = ydir;
        stackBase.style.left = xdir;
    }
}
