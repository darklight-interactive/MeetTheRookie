using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SynthesisObject : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SynthesisObject> { }

    SynthesisDraggable manipulator;

    public Label noteHeader;
    
    public SynthesisObject() {
        AddToClassList("synthesis-object");
        manipulator = new SynthesisDraggable(this);

        noteHeader = new Label();
        noteHeader.AddToClassList("label");
        Add(noteHeader);
    }
}
