using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

public class TextBubbleObject : UXML_RenderTextureObject
{
    TextBubble _textBubble;

    readonly string DEFAULT_TEXT = "This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

    [SerializeField, ShowOnly] string _currText;

    protected override void OnInitialized()
    {
        Debug.Log($"TextBubbleObject.OnInitialized()", this);
        base.OnInitialized();

        _textBubble = ElementQuery<TextBubble>();
        _textBubble.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            float fullTextHeight = evt.newRect.height;
            float fullTextWidth = evt.newRect.width;

            _textBubble.style.height = fullTextHeight;
            _textBubble.style.width = fullTextWidth;

            //_textBubble.SetFullText(currentText);
            //StartCoroutine(SpeechBubbleRollingTextRoutine(text, 0.025f));
        });

        int bubbleCount = ElementQueryAll<TextBubble>().ToList().Count;
        //SetText(DEFAULT_TEXT);
    }


    public void SetText(string text)
    {
        _textBubble.SetFullText(text);
        _textBubble.InstantCompleteText();
        _currText = text;

        TextureUpdate();
    }
}