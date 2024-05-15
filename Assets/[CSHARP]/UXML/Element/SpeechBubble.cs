using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SpeechBubble : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SpeechBubble> { }

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

        this.AddToClassList("speech-bubble:active");
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
