using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Toggleable : MonoBehaviour, IInteractable
{
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(){
        if(isOn){
            sr.sprite = off;
        }else{
            sr.sprite = on;
        }
        isOn = !isOn;
    }
}
