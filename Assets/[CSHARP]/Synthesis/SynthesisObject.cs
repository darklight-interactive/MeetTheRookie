using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SynthesisObject : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SynthesisObject> { }
    
    public SynthesisObject() {
        AddToClassList("synthesis-object");
    }

    public void Configure(object[] args) {
        Label noteHeader = new Label();
        if (args.Length > 0) {
            noteHeader.text = (string)args[0];
        }
        noteHeader.AddToClassList("label");
        Add(noteHeader);
    }
}
