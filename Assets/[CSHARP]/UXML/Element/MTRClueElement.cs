using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MTRClueElement : SelectableVisualElement
{
    const string UNDISCOVERED_CLASS = "undiscovered";

    const string CLUE_IMAGE_TAG = "clue-image";
    const string CLUE_TEXT_TAG = "clue-text";
    const int PADDING = 25;
    const int MARGIN = 25;

    //const int SIZE = 300;
    const int FONT_SIZE = 20;

    VisualElement _imageElement;
    TextElement _textElement;
    string _name = "default_name";
    Sprite _image;
    bool _isDiscovered = false;
    MTRInteractableDataSO _interactableData;
    Sprite _backgroundImage;

    [UxmlAttribute]
    public MTRInteractableDataSO InteractableData
    {
        get => _interactableData;
        set { SetInteractableData(value); }
    }

    [UxmlAttribute]
    public Sprite BackgroundImage
    {
        get => _backgroundImage;
        set { SetBackgroundImage(value); }
    }

    [UxmlAttribute]
    public bool IsDiscovered
    {
        get => _isDiscovered;
        set
        {
            _isDiscovered = value;
            if (!_isDiscovered)
            {
                _imageElement.AddToClassList(UNDISCOVERED_CLASS);
                _textElement.text = "???";
            }
            else
            {
                _imageElement.RemoveFromClassList(UNDISCOVERED_CLASS);
                _textElement.text = _name;
            }
        }
    }

    public MTRClueElement()
    {
        // << BASE STYLE >>
        this.style.position = Position.Relative;
        this.style.flexDirection = FlexDirection.Column;
        this.style.height = 300;

        SetPadding();
        SetMargin();
        SetBackgroundImage(_backgroundImage);

        // << CREATE CLUE IMAGE >>
        _imageElement = new VisualElement()
        {
            name = CLUE_IMAGE_TAG,
            style =
            {
                flexGrow = 1,
                alignSelf = Align.Center,
                minWidth = 150,
                maxWidth = 250,
                minHeight = 150,
                maxHeight = 300,
            },
        };
        SetImage(_image);
        this.Add(_imageElement);

        // << CREATE CLUE TEXT >>
        _textElement = new TextElement()
        {
            name = CLUE_TEXT_TAG,
            text = _name,
            style =
            {
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = FONT_SIZE,
                width = Length.Percent(100),
            }
        };
        this.Add(_textElement);
    }

    public MTRClueElement(MTRInteractableDataSO interactableData, Sprite backgroundImage)
        : this()
    {
        SetInteractableData(interactableData);
        SetBackgroundImage(backgroundImage);
    }

    void SetPadding()
    {
        this.style.paddingLeft = PADDING;
        this.style.paddingRight = PADDING;
        this.style.paddingTop = PADDING;
        this.style.paddingBottom = PADDING;
    }

    void SetMargin()
    {
        this.style.marginLeft = MARGIN;
        this.style.marginRight = MARGIN;
        this.style.marginTop = MARGIN;
        this.style.marginBottom = MARGIN;
    }

    void SetInteractableData(MTRInteractableDataSO interactableData)
    {
        _interactableData = interactableData;
        if (_interactableData == null)
            return;

        _name = interactableData.Name;
        _image = interactableData.Sprite;

        SetImage(_image);
        SetText(_name);
    }

    void SetBackgroundImage(Sprite backgroundImage)
    {
        _backgroundImage = backgroundImage;
        if (_backgroundImage != null)
        {
            this.style.backgroundImage = new StyleBackground(_backgroundImage);
            this.style.backgroundColor = Color.clear;
        }
        else
        {
            this.style.backgroundColor = Color.yellow;
        }
    }

    void SetImage(Sprite image)
    {
        _image = image;

        if (_image != null)
        {
            _imageElement.style.backgroundImage = new StyleBackground(_image);
            _imageElement.style.backgroundColor = Color.clear;
        }
        else
        {
            _imageElement.style.backgroundColor = Color.white;
        }
    }

    void SetText(string text)
    {
        _textElement.text = text;
    }
}
