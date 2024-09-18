using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Core2D;

[UxmlElement]
public partial class TextBubble : ControlledLabel
{
    Spatial2D.AnchorPoint _anchorPoint = Spatial2D.AnchorPoint.CENTER;
    Spatial2D.AnchorPoint _originPoint = Spatial2D.AnchorPoint.CENTER;

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

    //public VisualElement Container { get => FindOrCreateContainer(); }

    public TextBubble()
    {
        //FindOrCreateContainer();
        SetBackgroundSprite(BubbleSprite);
    }

    // Method to find the parent element with the ID 'container'

    private VisualElement FindOrCreateContainer()
    {
        // Try to find the container by traversing the hierarchy upwards
        VisualElement container = FindParentById(this, "container");

        // If not found, create a new container
        if (container == null)
        {
            container = new VisualElement { name = "container" };
            // Set up the container's styles or properties as needed
            container.style.flexDirection = FlexDirection.Column;

            // Add the new container to the root or desired parent
            // Assuming root or another parent context exists, adjust as necessary
            this.parent?.Add(container);
        }

        return container;
    }


    private VisualElement FindParentById(VisualElement element, string id)
    {
        // Traverse upwards to find a parent with the specified ID
        while (element != null)
        {
            if (element.name == id)
            {
                return element;
            }
            element = element.parent;
        }
        return null;
    }

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