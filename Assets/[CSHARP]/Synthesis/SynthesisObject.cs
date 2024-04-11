using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SynthesisObject : VisualElement
{
    SynthesisDraggable manipulator;
    
    public SynthesisObject() {
        manipulator = new SynthesisDraggable(this);
    }
}
