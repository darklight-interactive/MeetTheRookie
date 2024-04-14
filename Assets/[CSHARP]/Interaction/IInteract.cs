using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteract
{
    string ink_knot { get; set; }
    public int counter { get; set; }

    public virtual void Target() { }
    public virtual void Interact()
    {
        counter++;
    }
    public virtual void Reset()
    {
        counter = 0;
    }
}
