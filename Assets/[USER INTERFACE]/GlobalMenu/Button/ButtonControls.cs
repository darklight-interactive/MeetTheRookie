using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ButtonControls : MonoBehaviour
{
    VisualElement root;
    public VisualElement j;
    List<string> k = new List<string>();
    private int indexer = 0;
    bool lrbool;
    public Button currentbutton;
    public Button previousbutton;
    // Start is called before the first frame update
    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

    void OnEnable()
    {
        ListElements(root);
    }
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            lrbool = true;
            ChangeSelected();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            lrbool = false;
            ChangeSelected();
        }
        ///j = (Button)root.Q(className: "gmbutton").ElementAt(indexer);
        ///selectedbutton.button = root.Q<Button>("Button" + (indexer).ToString());
    }

    void ChangeSelected()
    {
        previousbutton = currentbutton;
        if (lrbool == true)
        {
            if (indexer < k.Count - 1)
            {
                indexer += 1;
            }
            else
            {
                indexer = 0;
            }
        }
        if (lrbool == false)
        {
            if (indexer > 0)
            {
                indexer -= 1;
            }
            else
            {
                indexer = k.Count - 1;
            }
        }
        currentbutton = root.Q<Button>(k[indexer]);
    }

    void ListElements(VisualElement element)
    {
        j = element;
        if (element.ClassListContains("gmbutton"))
        {
            k.Add(element.name);
        }
        foreach (var child in element.hierarchy.Children())
        {
            ListElements(child);
        }
    }
}
