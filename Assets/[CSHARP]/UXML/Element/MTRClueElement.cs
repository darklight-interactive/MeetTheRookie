using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MTRClueElement : VisualElement
{
    const string CLUE_IMAGE_TAG = "clue-image";
    const string CLUE_TEXT_TAG = "clue-text";

    string _name = "???";
    int _size = 100;
    Texture2D _backgroundImage;
    Texture2D _clueImage;

    VisualElement _clueImageElement;
    Label _clueTextElement;

    [UxmlAttribute]
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    [UxmlAttribute, Range(10, 500)]
    public int Size
    {
        get => _size;
        set
        {
            _size = value;
            this.style.width = _size;
            this.style.height = _size;
        }
    }

    [UxmlAttribute]
    public Texture2D BackgroundImage
    {
        get => _backgroundImage;
        set
        {
            _backgroundImage = value;
            this.style.backgroundImage = new StyleBackground(value);
        }
    }

    [UxmlAttribute]
    public Texture2D ClueImage
    {
        get => _clueImage;
        set
        {
            _clueImage = value;
            _clueImageElement.style.backgroundImage = new StyleBackground(value);
        }
    }

    public MTRClueElement()
    {
        // << BASE STYLE >>
        Size = this._size;
        this.style.position = Position.Relative;
        this.style.flexDirection = FlexDirection.Column;
        this.style.alignSelf = Align.Stretch;
        this.style.backgroundImage = new StyleBackground(BackgroundImage);

        // << CLUE IMAGE >>
        _clueImageElement = new VisualElement()
        {
            name = CLUE_IMAGE_TAG,
            style =
            {
                backgroundImage = _clueImage,
                flexGrow = 1,
                flexDirection = FlexDirection.Column,
                alignSelf = Align.Stretch,
            }
        };
        this.Add(_clueImageElement);

        // << CLUE TEXT >>
        _clueTextElement = new Label()
        {
            name = CLUE_TEXT_TAG,
            style = { color = Color.black, fontSize = 16, }
        };
        _clueTextElement.text = Name;
        this.Add(_clueTextElement);
    }

    public MTRClueElement(Texture2D backgroundImage, Texture2D clueImage, int size = 250)
        : this()
    {
        BackgroundImage = backgroundImage;
        ClueImage = clueImage;
        Size = size;
    }

    public new class UxmlFactory : UxmlFactory<MTRClueElement> { }
}
