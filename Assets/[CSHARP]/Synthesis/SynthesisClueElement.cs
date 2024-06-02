using UnityEngine.UIElements;

[UxmlElement]
public partial class SynthesisClueElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SynthesisClueElement> { }

    public Label noteHeader;

    public SynthesisClueElement()
    {
        AddToClassList("synthesis-object");

        noteHeader = new Label();
        noteHeader.AddToClassList("label");
        Add(noteHeader);
    }
}
