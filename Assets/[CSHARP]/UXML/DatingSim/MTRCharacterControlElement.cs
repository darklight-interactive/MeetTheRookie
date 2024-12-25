using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement]
public partial class MTRCharacterControlElement : VisualElement
{
    const string CHAR_CONTAINER_TAG = "character-container";
    const string CHAR_IMAGE_TAG = "character-image";
    const string CHAR_CONTAINER_CLASS = CHAR_CONTAINER_TAG;
    const string CHAR_IMAGE_CLASS = CHAR_IMAGE_TAG;
    const string ACTIVE_CLASS = "active";
    const string INACTIVE_CLASS = "inactive";


    bool _active = true;
    string _characterName = "unknown";
    Texture2D _characterImage;


    VisualElement _imageElement;


    [Header("(( Character Control Element )) ---- >>")]
    [UxmlAttribute]
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;

            SetActive(this, value);
            SetActive(_imageElement, value);

            Debug.Log($"{_characterName} is {(value ? "active" : "inactive")}");
        }
    }

    [UxmlAttribute]
    public string CharacterName
    {
        get => _characterName;
        set
        {
            _characterName = value;
            if (_imageElement != null)
                _imageElement.name = $"{CHAR_IMAGE_TAG}-{value}";
        }
    }

    [UxmlAttribute]
    public Texture2D CharacterImage
    {
        get => _characterImage;
        set
        {
            _characterImage = value;
            if (_imageElement != null)
                _imageElement.style.backgroundImage = value;
        }
    }

    public MTRCharacterControlElement()
    {
        this.AddToClassList(CHAR_CONTAINER_CLASS);
        CreateCharacterControlElement("lupe", out _imageElement);
    }

    void CreateCharacterControlElement(string characterName, out VisualElement imageElement)
    {

        // << CREATE CHARACTER IMAGE ELEMENT >>
        imageElement = new VisualElement()
        {
            style =
            {
                backgroundImage = CharacterImage,
            }
        };
        imageElement.name = $"{CHAR_IMAGE_TAG}-{characterName}";
        imageElement.AddToClassList(CHAR_IMAGE_CLASS);

        // << CREATE HEIRARCHY >>
        this.Add(imageElement);
    }

    void SetImage(VisualElement imageElement, Texture2D image)
    {
        imageElement.style.backgroundImage = image;
    }

    void SetActive(VisualElement element, bool active)
    {
        if (element == null)
            return;
        element.AddToClassList(active ? ACTIVE_CLASS : INACTIVE_CLASS);
        element.RemoveFromClassList(!active ? ACTIVE_CLASS : INACTIVE_CLASS);
    }

    public class CharacterControlElementFactory : UxmlFactory<MTRCharacterControlElement> { }
}