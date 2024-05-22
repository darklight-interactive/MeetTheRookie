using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SynthBigPaperUI : MonoBehaviour
{
    public VisualElement root;
    private VisualElement basefile;
    private VisualElement paper;
    private VisualElement selected;
    private Label label;
    private bool nothovered = true;
    private bool hovered;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        basefile = root.Q<VisualElement>("Base");
        paper = basefile.Q<VisualElement>("Paper");
        selected = basefile.Q<VisualElement>("Selected");
        label = basefile.Q<Label>("Label");
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered == true)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Select();
            }
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                OnHover();
            }
        }
        if (nothovered == true)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Debug.Log("working");
                OnHover();
            }
        }
        
    }
    void Select()
    {
        
    }
    void OnHover()
    {
        Debug.Log("jimmy");
        nothovered = !nothovered;
        hovered = !hovered;
        if (hovered == true)
        {
            selected.style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0.5372549f));
        }
        if (hovered == false)
        {
            selected.style.backgroundColor = new StyleColor(new Color(0.735849f, 0.6361687f, 0, 0));
        }
    }
    void PaperSelect()
    {

    }
}
