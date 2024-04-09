using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement]
public partial class UXML_InkyBubble : VisualElement
{
    Label inkyLabel;

    public UXML_InkyBubble()
    {
        AddToClassList("inky-bubble");

        // Create label
        inkyLabel = new Label
        {
            text = "UXML_InkyBubble.cs"
        };
        inkyLabel.AddToClassList("inky-label");
        this.Add(inkyLabel);

        this.visible = false;
    }

    public void SetText(string text)
    {
        inkyLabel.text = text;
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
