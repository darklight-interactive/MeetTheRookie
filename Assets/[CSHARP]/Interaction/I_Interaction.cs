using System;
using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;
public abstract class I_Interaction : MonoBehaviour
{
    UXML_InteractionUI interactionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    public string title = "Interaction";
    public string description = "This is an interaction";
    public Darklight.Console console { get; } = new Darklight.Console();
    public int counter { get; private set; } = 0;
    public void Interact()
    {
        counter++;
        console.Log("Interact >> ");
    }
}

/// <summary>
/// This is the base MonoBehaviour class for interactions
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BasicInteraction : I_Interaction
{
}
