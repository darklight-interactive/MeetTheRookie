using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UXML;
using Darklight.Selectable;

[UxmlElement]
public partial class SpeechBubble : SelectableVisualElement<VisualElement>
{
    public new class UxmlFactory : UxmlFactory<SpeechBubble> { }

    [UxmlAttribute("bubble-sprite")]
    public Sprite bubbleSprite;

    [UxmlAttribute("text")]
    public string text
    {
        get { return speechLabel.text; }
        set { speechLabel.text = value; }
    }

    public Label speechLabel = new Label
    {
        text = "New UXML Element Comic Bubble Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well."
    };

    public SpeechBubble()
    {
        this.Add(speechLabel);
        this.AddToClassList("speech-bubble");
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
