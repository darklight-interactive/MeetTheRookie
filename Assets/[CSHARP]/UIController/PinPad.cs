using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PinPad : MonoBehaviour
{
    public VisualElement groupbase;
    public Label Numbers;
    public VisualElement currentselected;
    public List<VisualElement> buttons = new List<VisualElement>();
    private int selector = 0;
    public StyleSheet selected;
    public string correctcode;
    public int Inputted = 0;
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        groupbase = root.Q<VisualElement>("Base");
        Numbers = root.Q<VisualElement>("Numbers").Q<Label>("Numbers");
        foreach (VisualElement row in groupbase.Children())
        { 
            foreach (VisualElement child in row.Children())
            {
                buttons.Add(child);
            }
        }
        correctcode = ("100722");
        Debug.Log(buttons.Count);
        buttons.Remove(Numbers);
        Debug.Log(buttons.Count);
        currentselected = buttons[selector];
        currentselected.RemoveFromClassList("UnHovered");
        currentselected.AddToClassList("Hovered");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selector < 11)
            {
            currentselected.RemoveFromClassList("Selected");
            currentselected.RemoveFromClassList("Hovered");
            currentselected.AddToClassList("UnHovered");
            selector += 1;
            currentselected = buttons[selector];
            currentselected.RemoveFromClassList("UnHovered");
            currentselected.AddToClassList("Hovered");
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selector < 9)
            {
            currentselected.RemoveFromClassList("Selected");
            currentselected.RemoveFromClassList("Hovered");
            currentselected.AddToClassList("UnHovered");
            selector += 3;
            currentselected = buttons[selector];
            currentselected.RemoveFromClassList("UnHovered");
            currentselected.AddToClassList("Hovered");
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selector > 0)
            {
            currentselected.RemoveFromClassList("Selected");
            currentselected.RemoveFromClassList("Hovered");
            currentselected.AddToClassList("UnHovered");
            selector -= 1;
            currentselected = buttons[selector];
            currentselected.RemoveFromClassList("UnHovered");
            currentselected.AddToClassList("Hovered");
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selector > 2)
            {
            currentselected.RemoveFromClassList("Selected");
            currentselected.RemoveFromClassList("Hovered");
            currentselected.AddToClassList("UnHovered");
            selector -= 3;
            currentselected = buttons[selector];
            currentselected.RemoveFromClassList("UnHovered");
            currentselected.AddToClassList("Hovered");
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Select());
        }
    }
    IEnumerator Select()
    {
        currentselected.RemoveFromClassList("Hovered");
        currentselected.AddToClassList("Selected");
        if (currentselected.name != "Pound" && currentselected.name != "Star")
        {
            if (Inputted < 6)
            {
            Inputted += 1;
            Numbers.text += currentselected.name;
            }
        }
        if (currentselected.name == "Star")
        {
            Inputted = 0;
            Numbers.text = "";
        }
        if (currentselected.name == "Pound")
        {
            if (Numbers.text != correctcode)
            {
                StartCoroutine(Incorrect());
            }
            if (Numbers.text == correctcode)
            {
                StartCoroutine(Correct());
            }
        }
        yield return new WaitForSecondsRealtime(0.3f);
        currentselected.RemoveFromClassList("Selected");
        currentselected.AddToClassList("Hovered");
    }
    IEnumerator Correct()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        groupbase.style.backgroundColor = new Color(0, .6f, .18f, .8f);
    }
    IEnumerator Incorrect()
    {
        groupbase.style.backgroundColor = new Color(.7f, .12f, 0f, 0.4f);
        yield return new WaitForSecondsRealtime(0.2f);
        groupbase.style.backgroundColor = new Color(0, .42f, .23f, .77f);
        yield return new WaitForSecondsRealtime(0.2f);
        groupbase.style.backgroundColor = new Color(.7f, .12f, 0f, 0.4f);
        yield return new WaitForSecondsRealtime(0.2f);
        groupbase.style.backgroundColor = new Color(0, .42f, .23f, .77f);
    }
}
