using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Core2D;
using NaughtyAttributes;
using UnityEditor.EditorTools;
using UnityEditor;

[UxmlElement]
public partial class TextBubble : ControlledLabel
{
    const string TAG = "TextBubble";

    VisualElement _bubbleContainer;

    TextBubbleLibrary _library;
    Spatial2D.AnchorPoint _originPoint = Spatial2D.AnchorPoint.CENTER;
    Spatial2D.AnchorPoint _directionPoint = Spatial2D.AnchorPoint.CENTER;



    [Header("[TEXT_BUBBLE] ================ >>>>")]
    [UxmlAttribute]
    public Spatial2D.AnchorPoint OriginPoint
    {
        get { return _originPoint; }
        set { AlignToOriginPoint(value); }
    }

    [UxmlAttribute]
    public TextBubbleLibrary Library
    {
        get { return _library; }
        set { _library = value; }
    }

    [UxmlAttribute, Tooltip("The directional anchor point of the bubble. Determines the alignment of the bubble and what sprite is used.")]
    public Spatial2D.AnchorPoint DirectionPoint
    {
        get { return _directionPoint; }
        set { AlignToDirectionPoint(value); }
    }

    public VisualElement bubbleContainer { get { return _bubbleContainer; } set { _bubbleContainer = value; } }

    public TextBubble()
    {
        this.style.flexGrow = 1;
        this.style.flexDirection = FlexDirection.Column;
        this.style.alignSelf = Align.Stretch;

        // << (( CREATE BUBBLE CONTAINER )) ---- >>>
        _bubbleContainer = new VisualElement
        {
            name = $"{TAG}-container",
            style =
            {
                flexDirection = FlexDirection.Row,
            }
        };
        //_bubbleContainer.Add(labelContainer);
        //this.Add(_bubbleContainer);
    }

    public void AlignBubble(Spatial2D.AnchorPoint originPoint, Spatial2D.AnchorPoint directionPoint)
    {
        AlignToOriginPoint(originPoint);
        AlignToDirectionPoint(directionPoint);
    }
    public void AlignBubble() => AlignBubble(_originPoint, _directionPoint);

    void AlignToOriginPoint(Spatial2D.AnchorPoint originPoint)
    {
        _originPoint = originPoint;
        switch (originPoint)
        {
            case Spatial2D.AnchorPoint.TOP_LEFT:
                this.style.justifyContent = Justify.FlexStart;
                this.style.alignItems = Align.FlexStart;
                break;
            case Spatial2D.AnchorPoint.TOP_CENTER:
                this.style.justifyContent = Justify.FlexStart;
                this.style.alignItems = Align.Center;
                break;
            case Spatial2D.AnchorPoint.TOP_RIGHT:
                this.style.justifyContent = Justify.FlexStart;
                this.style.alignItems = Align.FlexEnd;
                break;
            case Spatial2D.AnchorPoint.CENTER_LEFT:
                this.style.justifyContent = Justify.Center;
                this.style.alignItems = Align.FlexStart;
                break;
            case Spatial2D.AnchorPoint.CENTER:
                this.style.justifyContent = Justify.Center;
                this.style.alignItems = Align.Center;
                break;
            case Spatial2D.AnchorPoint.CENTER_RIGHT:
                this.style.justifyContent = Justify.Center;
                this.style.alignItems = Align.FlexEnd;
                break;
            case Spatial2D.AnchorPoint.BOTTOM_LEFT:
                this.style.justifyContent = Justify.FlexEnd;
                this.style.alignItems = Align.FlexStart;
                break;
            case Spatial2D.AnchorPoint.BOTTOM_CENTER:
                this.style.justifyContent = Justify.FlexEnd;
                this.style.alignItems = Align.Center;
                break;
            case Spatial2D.AnchorPoint.BOTTOM_RIGHT:
                this.style.justifyContent = Justify.FlexEnd;
                this.style.alignItems = Align.FlexEnd;
                break;

        }

    }

    void AlignToDirectionPoint(Spatial2D.AnchorPoint directionPoint)
    {

    }

    public new class UxmlFactory : UxmlFactory<TextBubble> { }

}