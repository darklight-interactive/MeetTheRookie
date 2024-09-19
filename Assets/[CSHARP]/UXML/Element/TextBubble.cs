using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Core2D;
using NaughtyAttributes;

[UxmlElement]
public partial class TextBubble : ControlledLabel
{
    VisualElement _container;
    ControlledLabel _controlledLabel;

    TextBubbleLibrary _library;
    Spatial2D.AnchorPoint _anchorPoint = Spatial2D.AnchorPoint.CENTER;
    Spatial2D.AnchorPoint _originPoint = Spatial2D.AnchorPoint.CENTER;

    [UxmlAttribute]
    public TextBubbleLibrary Library
    {
        get { return _library; }
        set { _library = value; }
    }

    [UxmlAttribute]
    public Sprite BubbleSprite
    {
        get { return this.style.backgroundImage.value.sprite; }
        set { SetBackgroundSprite(value); }
    }

    [UxmlAttribute]
    public Spatial2D.AnchorPoint AnchorPoint
    {
        get { return _anchorPoint; }
        set { SetAnchorPoint(value); }
    }

    [UxmlAttribute]
    public Spatial2D.AnchorPoint OriginPoint
    {
        get { return _originPoint; }
        set { SetOriginPoint(value); }
    }

    public TextBubble() { }

    public void SetAnchorPoint(Spatial2D.AnchorPoint anchorPoint)
    {
        _anchorPoint = anchorPoint;

        switch (anchorPoint)
        {
            case Spatial2D.AnchorPoint.TOP_LEFT:
            case Spatial2D.AnchorPoint.CENTER_LEFT:
            case Spatial2D.AnchorPoint.BOTTOM_LEFT:
                SetAlignSelf(Align.FlexStart);
                break;

            case Spatial2D.AnchorPoint.TOP_CENTER:
            case Spatial2D.AnchorPoint.CENTER:
            case Spatial2D.AnchorPoint.BOTTOM_CENTER:
                SetAlignSelf(Align.Center);
                break;

            case Spatial2D.AnchorPoint.TOP_RIGHT:
            case Spatial2D.AnchorPoint.CENTER_RIGHT:
            case Spatial2D.AnchorPoint.BOTTOM_RIGHT:
                SetAlignSelf(Align.FlexEnd);
                break;
        }
    }

    public void SetOriginPoint(Spatial2D.AnchorPoint originPoint)
    {
        _originPoint = originPoint;

        switch (originPoint)
        {
            case Spatial2D.AnchorPoint.TOP_LEFT:
            case Spatial2D.AnchorPoint.TOP_CENTER:
            case Spatial2D.AnchorPoint.TOP_RIGHT:
                SetContainerJustifyContent(Justify.FlexStart);
                break;

            case Spatial2D.AnchorPoint.CENTER_LEFT:
            case Spatial2D.AnchorPoint.CENTER:
            case Spatial2D.AnchorPoint.CENTER_RIGHT:
                SetContainerJustifyContent(Justify.Center);
                break;

            case Spatial2D.AnchorPoint.BOTTOM_LEFT:
            case Spatial2D.AnchorPoint.BOTTOM_CENTER:
            case Spatial2D.AnchorPoint.BOTTOM_RIGHT:
                SetContainerJustifyContent(Justify.FlexEnd);
                break;
        }
    }

    public void SetContainerJustifyContent(Justify justify)
    {
        //Container.style.justifyContent = justify;
    }

    public void SetAlignSelf(Align align)
    {
        this.style.alignSelf = align;
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }

    public new class UxmlFactory : UxmlFactory<TextBubble> { }

}