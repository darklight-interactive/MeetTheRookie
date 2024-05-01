using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;


public class UXML_ComicBubble : UXML_CustomElement
{
    const string label_class = "comic-label";
    const string bubble_class = "comic-bubble";

    Label label;
    public string Text
    {
        get => label.text;
        set => label.text = value;
    }

    public UXML_ComicBubble()
    {
        AddToClassList(bubble_class);

        // Create label
        label = new Label
        {
            text = "New UXML Element Comic Bubble Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well."
        };
        label.AddToClassList(label_class);
        this.Add(label);
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
