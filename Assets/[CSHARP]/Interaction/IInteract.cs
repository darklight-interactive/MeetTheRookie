using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteract
{
    string ink_knot { get; set; }
    Vector3 world_position { get; }
    public Darklight.Console console => new Darklight.Console();
    public int counter { get; set; }
    public virtual void Interact()
    {
        counter++;
    }
    public virtual void Reset()
    {
        counter = 0;
    }
}
