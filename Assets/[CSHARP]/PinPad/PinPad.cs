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
    public
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
        Debug.Log(buttons.Count);
        buttons.Remove(Numbers);
        Debug.Log(buttons.Count);
        currentselected = buttons[selector];
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selector < 11)
            {
                currentselected.style.backgroundColor = new Color(.4f, .57f, .32f, .8f);
                selector += 1;
                currentselected = buttons[selector];
                currentselected.style.backgroundColor = new Color(0.771f, .64f, 0f, .72f);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selector < 9)
            {
            currentselected.style.backgroundColor = new Color(.4f, .57f, .32f, .8f);
            selector += 3;
            currentselected = buttons[selector];
            currentselected.style.backgroundColor = new Color(0.771f, .64f, 0f, .72f);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selector > 0)
            {
            currentselected.style.backgroundColor = new Color(.4f, .57f, .32f, .8f);
            selector -= 1;
            currentselected = buttons[selector];
            currentselected.style.backgroundColor = new Color(0.771f, .64f, 0f, .72f);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selector > 2)
            {
            currentselected.style.backgroundColor = new Color(.4f, .57f, .32f, .8f);
            selector -= 3;
            currentselected = buttons[selector];
            currentselected.style.backgroundColor = new Color(0.771f, .64f, 0f, .72f);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentselected.RemoveFromClassList("Hovered");
            currentselected.AddToClassList("Selected");
            currentselected.style.backgroundColor = new Color(1f, .6f, 0f, .8f);
            if (currentselected.name != "Pound" && currentselected.name != "Star")
            {
                Numbers.text += currentselected.name + " ";
            }
            if (currentselected.name == "Star")
            {
                Numbers.text = "";
            }
            if (currentselected.name == "Pound")
            {
                Numbers.text = "B O O M";
            }
        }
    }
}
