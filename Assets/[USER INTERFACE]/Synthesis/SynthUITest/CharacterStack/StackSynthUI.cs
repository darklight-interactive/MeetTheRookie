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
    private float colorchanger;
    private string j;
    private bool selected = false;
    public bool hovered = false;
    public bool nothovered = true;
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
        foreach (VisualElement child in stackBase.Children())
            {
                Stack.Add(child);
                childcounter += 1;
                child.style.top = 20 * childcounter;
                child.style.left = 20 * childcounter;
                colorchanger = ((childcounter+1) * 2.5f)/10;
                child.style.unityBackgroundImageTintColor = new StyleColor(new Color(colorchanger, colorchanger, colorchanger, 1));
            }
        childcounter = 0;
        AddText("Irene", "yomama");
        AddText("Irene", "I'm Irene");
        AddText("Irene", "shiminy");
        AddText("Irene", "biminy");
        AddText("Irene", "bop");
        AddText("Irene", "the kids went down to the store");
        AddText("Irene", "hi my name is bob and I approve this message");
        AddText("Roy", "yomama");
        AddText("Roy", "I'm Roy");
        AddText("Roy", "shiminy");
        AddText("Roy", "biminy");
        AddText("Roy", "bop");
        AddText("Roy", "the kids went down to the store");
        AddText("Roy", "hi my name is bob and I approve this message");
        TextFix(Stack[^1].name);
        //Make sure to comment this out later!
    }

    // Update is called once per frame
    void Update()
    {
        CurrentFile = Stack[^1];
        if (hovered == true || textselected == true)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Select();
            }
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                Deselect();
            }
            if (Keyboard.current.cKey.wasPressedThisFrame && textselected != true)
            {
                Change();
            }
            if (textselected == true)
            {
                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    TextChange();
                }
            }
        }
        if (nothovered == true)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                OnHover();
            }
        }
    }

    public void Select()
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
    public void Deselect()
    {
        if (hovered == true)
        {
            OnHover();
        }
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
        CurrentFile.Q<VisualElement>("Hovered").style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
        holder = Stack[0];
        Stack.Remove(Stack[0]);
        Stack.Add(holder);
        foreach (VisualElement child in Stack)
        {   
            Debug.Log(childcounter);
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
}
