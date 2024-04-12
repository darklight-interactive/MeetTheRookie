using System;
using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;
public abstract class I_Interaction : MonoBehaviour
{
    public string title = "Interaction";
    public string description = "This is an interaction";
    public Darklight.Console console { get; } = new Darklight.Console();
    public int counter { get; private set; } = 0;
    public virtual void Interact()
    {
        counter++;
        Debug.Log($"Interact >> {counter}");
    }
}
