using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement]
public partial class UXMLElement_ComicBubble : VisualElement
{
    const string label_class = "comic-label";
    const string bubble_class = "comic-bubble";

    Label label;

    public UXMLElement_ComicBubble()
    {
        AddToClassList(bubble_class);

        // Create label
        label = new Label
        {
            text = "New UXMLElement_ComicBubble Label"
        };
        label.AddToClassList(label_class);
        this.Add(label);
    }

    public void SetText(string text)
    {
        label.text = text;
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
