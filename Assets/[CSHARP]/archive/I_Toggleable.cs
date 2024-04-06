using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class I_Toggleable : MonoBehaviour, IInteractable
{
    public bool isLocked = false;
    public string prefix = "((TOGGLEABLE)) ";

    public Sprite on;
    public Sprite off;

    private SpriteRenderer sr;

    private bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = off;
    }

    public void Interact(){

        Debug.Log(prefix + " called interact");

        if(isOn)
        {
            sr.sprite = off;
        }
        else
        {
            sr.sprite = on;
        }
        isOn = !isOn;
    }
    
}
