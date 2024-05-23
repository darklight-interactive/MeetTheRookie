using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SynthesisItems : MonoBehaviour
{
    public VisualElement root;
    private VisualElement basefile;
    private VisualElement hoverset;
    private Label text;
    private SynthStackUI stack;
    public SynthTestControls controls;
    private bool stacked;
    public bool hovered = false;
    public bool selected = false;
    public bool synthbutton = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        controls = FindAnyObjectByType<SynthTestControls>();
        if (gameObject.tag == "Stack")
        {
            Debug.Log("stack");
            stacked = true;
            stack = gameObject.GetComponent<SynthStackUI>();
            hoverset = stack.Stack[^1].Q<VisualElement>("Hovered");
            text = stack.CurrentLine;
        }
        root = GetComponent<UIDocument>().rootVisualElement;
        basefile = root.Q<VisualElement>("Base");
        if (stacked == false)
        {
            Debug.Log(gameObject.name);
            hoverset = basefile.Q<VisualElement>("Selected");
            text = basefile.Q<Label>("Text");
        }
        if (gameObject.name == "SynthesizeButton")
        {
            synthbutton = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered == true && selected == true)
        {
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                Deselect();
            }
        }
        if (hovered == true && stacked == false)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Select();
            }
        }
        if (hovered == true && stacked == true)
        {
            stack.CurrentFile = stack.Stack[^1];

            if (hovered == true || stack.textselected == true)
            {
                if (Keyboard.current.zKey.wasPressedThisFrame)
                {
                    stack.Select();
                }
                if (Keyboard.current.xKey.wasPressedThisFrame)
                {
                    stack.Deselect();
                }
                if (Keyboard.current.cKey.wasPressedThisFrame && stack.textselected != true)
                {
                    stack.Change();
                }
                if (stack.textselected == false)
                {
                    controls.movable = true;
                }
                if (stack.textselected == true)
                {
                    controls.movable = false;
                    if (Keyboard.current.cKey.wasPressedThisFrame)
                    {
                        stack.TextChange();
                    }
                }
            }
        }
    }
    public void Select()
    {
        if (synthbutton == false)
        {
            selected = true;
            //somelist.add(this.gameObject);
            ChangeText("Selected!");
        }
        /*if (synthbutton == true)
        {
            controls.Combine();
        }*/
    }
    public void Deselect()
    {
        selected = false;
        ChangeText("Not Selected Anymore");
    }
    public void ChangeText(string newtext)
    {
        text.text = newtext;
    }
    public void OnHover()
    {
        if (stacked == false)
        {
            hovered = !hovered;
            if (hovered == true)
            {
                hoverset.style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
            }
            if (hovered == false)
            {
                hoverset.style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
            }
        }
        if (stacked == true)
        {
            hovered = !hovered;
            stack.OnHover();
        }
    }
}
