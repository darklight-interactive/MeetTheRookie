using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MTRClueElement : VisualElement
{
    const string UNDISCOVERED_CLASS = "undiscovered";

    const string CLUE_IMAGE_TAG = "clue-image";
    const string CLUE_TEXT_TAG = "clue-text";

    MTRClueLibrary _library;
    MTRInteractableDataSO _clueData;
    VisualElement _imageElement;
    TextElement _textElement;
    int _padding = 10;
    int _size = 100;
    string _name = "???";
    bool _isDiscovered = false;

    [UxmlAttribute]
    public MTRInteractableDataSO ClueData
    {
        get => _clueData;
        set
        {
            _clueData = value;
            if (_clueData == null)
            {
                _imageElement.style.backgroundImage = null;
                _imageElement.style.backgroundColor = Color.white;
                _textElement.text = "???";
            }
            else
            {
                _imageElement.style.backgroundImage = new StyleBackground(_clueData.Sprite);
                _imageElement.style.backgroundColor = Color.clear;

                _textElement.text = _clueData.name;
            }
        }
    }

    [UxmlAttribute, Range(0, 100)]
    public int Padding
    {
        get => _padding;
        set { SetPadding(value); }
    }

    [UxmlAttribute, Range(100, 1000)]
    public int Size
    {
        get => _size;
        set { SetSize(value); }
    }

    [UxmlAttribute]
    public bool IsDiscovered
    {
        get => _isDiscovered;
        set
        {
            _isDiscovered = value;
            if (_isDiscovered)
            {
                _imageElement.AddToClassList(UNDISCOVERED_CLASS);
                _textElement.text = "???";
            }
            else
            {
                _imageElement.RemoveFromClassList(UNDISCOVERED_CLASS);
                _textElement.text = _clueData.name;
            }
        }
    }

    public MTRClueElement()
    {
        // << BASE STYLE >>
        this.style.position = Position.Relative;
        this.style.flexDirection = FlexDirection.Column;
        this.style.alignSelf = Align.Stretch;
        this.style.backgroundColor = Color.yellow;

        SetPadding(_padding);
        SetSize(_size);

        // << CREATE CLUE IMAGE >>
        _imageElement = new VisualElement();
        _imageElement.name = CLUE_IMAGE_TAG;
        _imageElement.style.backgroundColor = Color.white;
        _imageElement.style.minWidth = new StyleLength(Length.Percent(80));
        _imageElement.style.minHeight = new StyleLength(Length.Percent(80));

        this.Add(_imageElement);

        // << CREATE CLUE TEXT >>
        _textElement = new TextElement();
        _textElement.name = CLUE_TEXT_TAG;
        _textElement.text = _name;
        _textElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        this.Add(_textElement);
    }

    public void SetPadding(int padding)
    {
        _padding = padding;
        this.style.paddingLeft = _padding;
        this.style.paddingRight = _padding;
        this.style.paddingTop = _padding;
        this.style.paddingBottom = _padding;
    }

    public void SetSize(int size)
    {
        _size = size;
        this.style.width = _size;
        this.style.height = _size;
    }
}
